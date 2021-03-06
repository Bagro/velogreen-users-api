using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VeloGreen.Users.Api.Entities;
using VeloGreen.Users.Api.Handlers;

namespace VeloGreen.Users.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationHandler _authenticationHandler;

        public AuthenticationController(IAuthenticationHandler authenticationHandler)
        {
            _authenticationHandler = authenticationHandler;
        }
        
        /// <summary>
        /// User authentication
        /// </summary>
        /// <param name="authenticationRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            try
            {
                return Ok(await _authenticationHandler.Authenticate(authenticationRequest));
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
            }
        }
    }
}
