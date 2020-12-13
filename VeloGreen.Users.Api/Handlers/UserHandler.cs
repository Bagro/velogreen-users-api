using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using VeloGreen.Users.Api.Entities;
using VeloGreen.Users.Api.Exceptions;
using VeloGreen.Users.Api.Storage;

namespace VeloGreen.Users.Api.Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly IUserRepository _userRepository;

        public UserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        public async Task<string> Authenticate(AuthenticationRequest authenticationRequest)
        {
            var user = await _userRepository.GetByEmail(authenticationRequest.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(authenticationRequest.Password, user.Password))
            {
                throw new AuthenticationException();
            }

            return GenerateJwtToken(user);
        }

        public Task<User> GetUserByEmail(string email)
        {
            return _userRepository.GetByEmail(email);
        }

        private static string GenerateJwtToken(User user)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Replace with key from settings that is long and 256"));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha384Signature);
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim("Admin", false.ToString())
                }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = signingCredentials,
                Issuer = "https://velogreen.se",
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }
    }
}
