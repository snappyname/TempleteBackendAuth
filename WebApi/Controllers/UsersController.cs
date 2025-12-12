using Application.Abstract;
using Dal;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateWebApi.Controllers.Base;

namespace TemplateWebApi.Controllers;

[ApiController]
[Route("users")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(AppDbContext context, IUserService userService) : base(context)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
        return Ok(await _userService.Login(loginModel.Email, loginModel.Password));
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        return Ok(await _userService.RefreshToken(refreshToken));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel registerModel)
    {
        return Ok(await _userService.Register(registerModel));
    }

    [HttpGet("me")]
    public IActionResult GetMe()
    {
        return Ok(_userService.GetMe(UserId));
    }
}