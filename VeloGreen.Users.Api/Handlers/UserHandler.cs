using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VeloGreen.Users.Api.Constants;
using VeloGreen.Users.Api.Entities;
using VeloGreen.Users.Api.Exceptions;
using VeloGreen.Users.Api.Storage;

namespace VeloGreen.Users.Api.Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Register(RegisterRequest registerRequest)
        {
            if (await _userRepository.IsEmailUsed(registerRequest.Email))
            {
                throw new DuplicateNameException();
            }

            User user = new()
            {
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password)
            };

            await _userRepository.Save(user);
        }

        public async Task Update(UpdateUserRequest updateUserRequest)
        {
            var user = await _userRepository.GetById(updateUserRequest.Id);

            if (user == null)
            {
                throw new UserNotFoundException($"User with id {updateUserRequest.Id} could not be found");
            }

            user.Id = updateUserRequest.Id;
            user.FirstName = updateUserRequest.FirstName;
            user.LastName = updateUserRequest.LastName;

            if (!string.IsNullOrWhiteSpace(updateUserRequest.CurrentPassword) && 
                !string.IsNullOrWhiteSpace(updateUserRequest.NewPassword) && 
                BCrypt.Net.BCrypt.Verify(user.Password, updateUserRequest.CurrentPassword))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserRequest.NewPassword);
            }

            await _userRepository.Save(user);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            if (CanAccess(ClaimConstants.Email, email) || CanAccess(ClaimConstants.Admin, true.ToString()))
            {
                return await _userRepository.GetByEmail(email);
            }

            throw new UnauthorizedAccessException();
        }

        private bool CanAccess(string type, string value)
        {
            return _httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type.Equals(type) && x.Value.Equals(value));
        }
        
    }
}
