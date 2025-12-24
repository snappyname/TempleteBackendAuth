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
    private readonly IGoogleAuthService _googleAuthService;

    public UsersController(AppDbContext context, IUserService userService, IGoogleAuthService googleAuthService) :
        base(context)
    {
        _userService = userService;
        _googleAuthService = googleAuthService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
        return Ok(await _userService.Login(loginModel.Email, loginModel.Password));
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken(RefreshTokenModel refreshTokenModel)
    {
        return Ok(await _userService.RefreshToken(refreshTokenModel.RefreshToken));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel registerModel)
    {
        return Ok(await _userService.Register(registerModel));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        return Ok(await _userService.GetMe(UserId));
    }

    [HttpPost("auth/google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        return Ok(await _googleAuthService.LoginByGoogle(request));
    }
}
