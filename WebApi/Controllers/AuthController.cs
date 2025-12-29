using Application.Abstract;
using Dal;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateWebApi.Controllers.Base;

namespace TemplateWebApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : BaseController
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IEmailAuthService _emailAuthService;
    private readonly IGithubAuthService _githubAuthService;

    public AuthController(AppDbContext context, IEmailAuthService emailAuthService, IGoogleAuthService googleAuthService, IGithubAuthService githubAuthService) : base(context)
    {
        _emailAuthService = emailAuthService;
        _googleAuthService = googleAuthService;
        _githubAuthService = githubAuthService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
        return Ok(await _emailAuthService.Login(loginModel.Email, loginModel.Password));
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken(RefreshTokenModel refreshTokenModel)
    {
        return Ok(await _emailAuthService.RefreshToken(refreshTokenModel.RefreshToken));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel registerModel)
    {
        return Ok(await _emailAuthService.Register(registerModel));
    }
    
    [AllowAnonymous]
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] OAuthTokenModel request)
    {
        return Ok(await _googleAuthService.LoginByGoogle(request));
    } 
    
    [AllowAnonymous]
    [HttpPost("github")]
    public async Task<IActionResult> GithubLogin([FromBody] OAuthTokenModel request)
    {
        return Ok(await _githubAuthService.LoginByGithub(request));
    }
}
