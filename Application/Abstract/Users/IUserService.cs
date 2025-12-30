using Domain;

namespace Application.Abstract.Users;

public interface IUserService
{
    Task<User> GetMe(string userId);
}
