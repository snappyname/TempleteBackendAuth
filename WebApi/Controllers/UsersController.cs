using Application.Abstract;
using Dal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateWebApi.Controllers.Base;

namespace TemplateWebApi.Controllers;

[ApiController]
[Route("users")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(AppDbContext context, IUserService userService) :
        base(context)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        return Ok(await _userService.GetMe(UserId));
    }
}
