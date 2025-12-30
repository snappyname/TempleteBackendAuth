using DTO;

namespace Application.Abstract.Auth
{
    public interface IGoogleAuthService
    {
        Task<TokensModel> LoginByGoogle(OAuthTokenModel request);
    }
}
