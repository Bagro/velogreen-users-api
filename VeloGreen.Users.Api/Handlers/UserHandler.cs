using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VeloGreen.Users.Api.Constants;
using VeloGreen.Users.Api.Entities;
using VeloGreen.Users.Api.Exceptions;
using VeloGreen.Users.Api.Storage;
using VeloGreen.Users.Api.Verifiers;

namespace VeloGreen.Users.Api.Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccessVerifier _accessVerifier;

        public UserHandler(IUserRepository userRepository, IAccessVerifier accessVerifier)
        {
            _userRepository = userRepository;
            _accessVerifier = accessVerifier;
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
            if (!_accessVerifier.HaveAccess(ClaimConstants.NameIdentifier, updateUserRequest.Id.ToString()) && !_accessVerifier.HaveAccess(ClaimConstants.Admin, true.ToString()))
            {
                throw new UnauthorizedAccessException();
            }
            
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
                BCrypt.Net.BCrypt.Verify(updateUserRequest.CurrentPassword, user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserRequest.NewPassword);
            }

            await _userRepository.Save(user);
        }

        public async Task<User> GetUserById(Guid id)
        {
            if (_accessVerifier.HaveAccess(ClaimConstants.NameIdentifier, id.ToString()) || _accessVerifier.HaveAccess(ClaimConstants.Admin, true.ToString()))
            {
                var user = await _userRepository.GetById(id);

                user.Password = string.Empty;
                return user;
            }

            throw new UnauthorizedAccessException();
        }
    }
}
