using DTO;

namespace Application.Abstract
{
    public interface IGithubAuthService
    {
        Task<TokensModel> LoginByGithub(OAuthTokenModel request);
    }
}
