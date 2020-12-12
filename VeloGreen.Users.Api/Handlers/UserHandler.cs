using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using VeloGreen.Users.Api.Entities;
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

        public async Task<string> Authenticate(AuthenticationRequest authenticationRequest)
        {
            var user = await _userRepository.GetByEmail(authenticationRequest.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(authenticationRequest.Password, user.Password))
            {
                throw new AuthenticationException();
            }

            return GenerateJwtToken(user.Id);
        }

        private static string GenerateJwtToken(Guid userId)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Replace with key from settings"));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha384Signature);
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()) }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = signingCredentials,
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }
    }
}
