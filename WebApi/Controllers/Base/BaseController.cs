using Dal;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace TemplateWebApi.Controllers.Base;

public abstract class BaseController(AppDbContext context) : ControllerBase, IAsyncActionFilter
{
    protected string UserId { get; private set; }
    protected User User { get; private set; }

    [NonAction]
    public async Task OnActionExecutionAsync(ActionExecutingContext context1, ActionExecutionDelegate next)
    {
        var principal = context1.HttpContext?.User;
        var idValue = principal?.FindFirst(UserClaimTypes.UserId)?.Value;

        if (!string.IsNullOrEmpty(idValue))
        {
            UserId = idValue;
            User = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idValue);
        }
        else
        {
           
        }

        await next();
    }
}