using Application.Abstract;
using Dal;
using Domain;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace Application.Services.UserAuth
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public GoogleAuthService(UserManager<User> userManager, IConfiguration configuration, AppDbContext context,
            HttpClient httpClient)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dbContext = context;
            _httpClient = httpClient;
        }

        public async Task<TokensModel> LoginByGoogle(GoogleLoginRequest request)
        {
            GoogleTokenResponse bearerToken = await ExchangeCodeAsync(request.IdToken);

            string userSubject = string.Empty;
            string userEmail = string.Empty;
            CreateGooglePayload(bearerToken.IdToken, ref userSubject, ref userEmail);
            User user = await FindOrCreateGoogleUser(userSubject, userEmail);
            string accessToken = TokenGenerator.CreateJwtToken(user, _configuration[ConfigurationKeys.JwtKey]!);
            RefreshToken refreshToken = await TokenGenerator.GenerateRefreshToken(user.Id, _dbContext);
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new TokensModel { JWTToken = accessToken, RefreshToken = refreshToken.Token };
        }

        private void CreateGooglePayload(string idToken, ref string userSubject, ref string userEmail)
        {
            JwtSecurityTokenHandler handler = new();

            JwtSecurityToken? jwt = handler.ReadJwtToken(idToken);

            if ((jwt.Issuer != "https://accounts.google.com" && jwt.Issuer != "accounts.google.com")
                || !jwt.Audiences.Contains(_configuration[ConfigurationKeys.GoogleClientId])
                || jwt.ValidTo < DateTime.UtcNow)
            {
                throw new SecurityTokenException();
            }

            userSubject = jwt.Subject;
            userEmail = jwt.Claims.First(x => x.Type == GoogleClaimTypes.UserEmail).Value;
        }

        private async Task<User> FindOrCreateGoogleUser(string googleId, string email)
        {
            User? login = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x =>
                x.Provider == AuthProvider.Google && x.GoogleId == googleId);
            if (login != null)
            {
                return login;
            }

            User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                user.GoogleId = googleId;
                user.Provider = AuthProvider.Google;
                await _dbContext.SaveChangesAsync();
                return user;
            }

            User newUser = new()
            {
                UserName = email,
                Email = email,
                Provider = AuthProvider.Google,
                GoogleId = googleId,
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(newUser);
            return newUser;
        }

        private async Task<GoogleTokenResponse> ExchangeCodeAsync(string code)
        {
            HttpResponseMessage response = await _httpClient.PostAsync(
                _configuration[ConfigurationKeys.GoogleRequestUrl],
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = _configuration[ConfigurationKeys.GoogleClientId]!,
                    ["client_secret"] = _configuration[ConfigurationKeys.GoogleClientSecret]!,
                    ["code"] = code,
                    ["grant_type"] = _configuration[ConfigurationKeys.GoogleGrantType]!,
                    ["redirect_uri"] = _configuration[ConfigurationKeys.GoogleRedirectUrl]!
                }));

            return (await response.Content.ReadFromJsonAsync<GoogleTokenResponse>())!;
        }
    }
}
