using DTO;

namespace Application.Abstract.Auth
{
    public interface IEmailAuthService
    {
        Task<TokensModel> Login(string email, string password);
        Task<TokensModel> RefreshToken(string refreshToken);
        Task<TokensModel> Register(RegisterModel model);
    }
}
