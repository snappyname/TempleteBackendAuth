using Dal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateWebApi.Controllers.Base;

namespace TemplateWebApi.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : BaseController
    {
        private readonly AppDbContext _dbContext;

        public HealthController(AppDbContext context) : base(context)
        {
            _dbContext = context;
        }

        [AllowAnonymous]
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("db")]
        public async Task<IActionResult> GetDbStatus()
        {
            if (await _dbContext.Users.AnyAsync())
            {
                return Ok();
            }

            return NotFound();
        }
    }
}
