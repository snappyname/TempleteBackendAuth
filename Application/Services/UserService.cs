using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstract;
using Application.Exceptions;
using Dal;
using Domain;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;

    public UserService(UserManager<User> userManager, IConfiguration configuration, AppDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _dbContext = context;
    }

    public async Task<TokensModel> Login(string email, string password)
    {
        var user = await _userManager.FindByNameAsync(email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            throw new LoginOrPasswordInvalidException();
        }

        var jwtToken = CreateJwtToken(user);
        var refreshToken = CreateRefreshToken(user.Id);

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new TokensModel { JWTToken = jwtToken, RefreshToken = refreshToken.Token };
    }

    public async Task<TokensModel> RefreshToken(string token)
    {
        var refreshToken = await _dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);
        if (refreshToken == null || refreshToken.IsRevoked)
        {
            throw new RefreshTokenInvalid();
        }

        refreshToken.IsRevoked = true;
        var newJwtToken = CreateJwtToken(refreshToken.User);
        var newRefreshToken = CreateRefreshToken(refreshToken.UserId);
        await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
        await _dbContext.SaveChangesAsync();
        return new TokensModel { JWTToken = newJwtToken, RefreshToken = newRefreshToken.Token };
    }

    public async Task<TokensModel> Register(RegisterModel model)
    {
        var newUser = new User { UserName = model.Email, Email = model.Email };
        await _userManager.CreateAsync(newUser, model.Password);
        return await Login(newUser.Email, model.Password);
    }

    public async Task<User> GetMe(string userId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
    }

    private string CreateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.UserName)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(60),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static RefreshToken CreateRefreshToken(string userId)
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(30),
            UserId = userId
        };
    }
}