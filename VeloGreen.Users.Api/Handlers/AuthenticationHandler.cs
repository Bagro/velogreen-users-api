using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VeloGreen.Users.Api.Constants;
using VeloGreen.Users.Api.Entities;
using VeloGreen.Users.Api.Entities.Settings;
using VeloGreen.Users.Api.Storage;

namespace VeloGreen.Users.Api.Handlers
{
    public class AuthenticationHandler : IAuthenticationHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthenticationSettings _authenticationSettings;

        public AuthenticationHandler(IOptions<AuthenticationSettings> options, IUserRepository userRepository)
        {
            _authenticationSettings = options.Value;
            _userRepository = userRepository;
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

        private string GenerateJwtToken(User user)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.SecurityKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha384Signature);
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(ClaimConstants.Admin, false.ToString())
                    }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = signingCredentials,
                Issuer = _authenticationSettings.Issuer,
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }
    }
}
