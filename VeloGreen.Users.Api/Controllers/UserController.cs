using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VeloGreen.Users.Api.Entities;
using VeloGreen.Users.Api.Exceptions;
using VeloGreen.Users.Api.Handlers;

namespace VeloGreen.Users.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _userHandler;

        public UserController(IUserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="registerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                await _userHandler.Register(registerRequest);
            }
            catch (DuplicateNameException)
            {
                return BadRequest();
            }

            return NoContent();
        }

        /// <summary>
        /// User update
        /// </summary>
        /// <param name="updateUserRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest updateUserRequest)
        {
            try
            {
                await _userHandler.Update(updateUserRequest);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Get user information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserByEmail(Guid id)
        {
            try
            {
                return Ok(await _userHandler.GetUserById(id));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
