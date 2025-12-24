using DTO;

namespace Application.Abstract
{
    public interface IGoogleAuthService
    {
        Task<TokensModel> LoginByGoogle(GoogleLoginRequest request);
    }
}
