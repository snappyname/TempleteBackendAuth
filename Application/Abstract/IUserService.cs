using Domain;
using DTO;

namespace Application.Abstract;

public interface IUserService
{
    Task<User> GetMe(string userId);
}
