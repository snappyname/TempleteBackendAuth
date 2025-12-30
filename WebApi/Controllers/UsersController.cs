using Application;
using Application.Abstract;
using Application.Abstract.SignalR;
using Application.Abstract.Users;
using Application.Services.SignalR;
using Dal;
using DTO;
using DTO.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateWebApi.Controllers.Base;

namespace TemplateWebApi.Controllers;

[ApiController]
[Route("users")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly IBroadcastService _broadcastService;

    public UsersController(AppDbContext context, IUserService userService, IBroadcastService broadcastService) :
        base(context)
    {
        _userService = userService;
        _broadcastService = broadcastService;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        return Ok(await _userService.GetMe(UserId));
    }  
    
    [AllowAnonymous]
    [HttpGet("testSignalR")]
    public async Task<IActionResult> TestSignalR()
    {
        await _broadcastService.SendToUserAsync(
            userId: UserId,
            messageType: BroadcastMessageTypes.TestMessage,
            payload: new TestModel() { OrderId = 42 }
        );
        return Ok();
    }
}
