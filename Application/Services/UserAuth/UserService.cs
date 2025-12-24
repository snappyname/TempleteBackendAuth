using Application.Abstract;
using Application.Exceptions;
using Dal;
using Domain;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Services.UserAuth;

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

        var jwtToken = TokenGenerator.CreateJwtToken(user, _configuration[ConfigurationKeys.JwtKey]!);
        var refreshToken = await TokenGenerator.GenerateRefreshToken(user.Id, _dbContext);
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
        var newJwtToken = TokenGenerator.CreateJwtToken(refreshToken.User, _configuration[ConfigurationKeys.JwtKey]!);
        var newRefreshToken = await TokenGenerator.GenerateRefreshToken(refreshToken.UserId, _dbContext);
        await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
        await _dbContext.SaveChangesAsync();
        return new TokensModel { JWTToken = newJwtToken, RefreshToken = newRefreshToken.Token };
    }

    public async Task<TokensModel> Register(RegisterModel model)
    {
        if (await _dbContext.Users.AnyAsync(x => x.Email == model.Email))
        {
            throw new UserWithThisEmailExist();
        }

        var newUser = new User { UserName = model.Email, Email = model.Email };
        await _userManager.CreateAsync(newUser, model.Password);
        return await Login(newUser.Email, model.Password);
    }

    public async Task<User> GetMe(string userId)
    {
        return (await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId))!;
    }
}
