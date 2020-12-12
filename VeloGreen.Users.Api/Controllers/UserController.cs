using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VeloGreen.Users.Api.Entities;
using VeloGreen.Users.Api.Handlers;

namespace VeloGreen.Users.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _userHandler;

        public UserController(IUserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            await _userHandler.Register(registerRequest);

            return NoContent();
        }
    }
}
