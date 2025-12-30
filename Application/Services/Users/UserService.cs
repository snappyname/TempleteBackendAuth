using Application.Abstract;
using Application.Abstract.Users;
using Dal;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Users;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Domain.User> GetMe(string userId)
    {
        return (await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId))!;
    }
}
