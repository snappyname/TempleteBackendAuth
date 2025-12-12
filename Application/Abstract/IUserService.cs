using Domain;
using DTO;

namespace Application.Abstract;

public interface IUserService
{
    Task<TokensModel> Login(string email, string password);
    Task<TokensModel> RefreshToken(string refreshToken);
    Task<TokensModel> Register(RegisterModel model);
    Task<User> GetMe(string userId);
}