using Application.Abstract;
using Application.Abstract.Auth;
using Application.Services.Auth.Extensions;
using Dal;
using Domain;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Application.Services.Auth
{
    public class GithubAuthService : IGithubAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public GithubAuthService(UserManager<User> userManager, IConfiguration configuration, AppDbContext context, HttpClient httpClient)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dbContext = context;
            _httpClient = httpClient;
        }
        
        public async Task<TokensModel> LoginByGithub(OAuthTokenModel request)
        {
            var token = await GetGithubToken(request.IdToken);
            var userLogin = await GetUserLogin(token);
            var email = await GetUserEmail(token);
            User user = await FindOrCreateGithubUser(userLogin.Id, userLogin.Login, email);
            string accessToken = TokenGenerator.CreateJwtToken(user, _configuration[ConfigurationKeys.JwtKey]!);
            RefreshToken refreshToken = await TokenGenerator.GenerateRefreshToken(user.Id, _dbContext);
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
            return new TokensModel { JWTToken = accessToken, RefreshToken = refreshToken.Token };
        }
        
        private async Task<string> GetGithubToken(string code)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _configuration[ConfigurationKeys.GithubClientId]!,
                ["client_secret"] = _configuration[ConfigurationKeys.GithubClientSecret]!,
                ["code"] = code,
                ["redirect_uri"] = _configuration[ConfigurationKeys.GithubRedirectUrl]!
            });

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            return (await response.Content.ReadFromJsonAsync<GithubTokenResponseModel>())!.AccessToken;
        }

        private async Task<GithubUserResponse> GetUserLogin(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.UserAgent.ParseAdd(_configuration[ConfigurationKeys.GithubAppName]!);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            return (await response.Content.ReadFromJsonAsync<GithubUserResponse>())!;
        }     
        
        private async Task<string> GetUserEmail(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.UserAgent.ParseAdd(_configuration[ConfigurationKeys.GithubAppName]!);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            return ((await response.Content.ReadFromJsonAsync<List<GithubEmailModel>>())?.Find(x => x.Primary)!).Email;
        }
        
        private async Task<User> FindOrCreateGithubUser(long githubId, string username, string email)
        {
            User? login = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.GithubId == githubId);
            if (login != null)
            {
                return login;
            }
            
            User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                user.GithubId = githubId;
                await _dbContext.SaveChangesAsync();
                return user;
            }
            
            User newUser = new()
            {
                UserName = username,
                Email =  email,
                GithubId = githubId,
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(newUser);
            return newUser;
        }
    }
}
