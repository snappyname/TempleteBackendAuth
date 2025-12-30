using DTO;

namespace Application.Abstract.Auth
{
    public interface IGithubAuthService
    {
        Task<TokensModel> LoginByGithub(OAuthTokenModel request);
    }
}
