using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using VeloGreen.Users.Api.Entities;
using VeloGreen.Users.Api.Entities.Settings;
using VeloGreen.Users.Api.Handlers;
using VeloGreen.Users.Api.Storage;
using Xunit;

namespace VeloGreen.Users.Api.Tests.Handlers
{
    public class AuthenticationHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IOptions<AuthenticationSettings> _authenticationSettingsOptions;
        private readonly AuthenticationHandler _authenticationHandler;

        public AuthenticationHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _authenticationSettingsOptions = Options.Create(
                new AuthenticationSettings
                {
                    Issuer = "https://issuer.test", 
                    SecurityKey = "The securitykey that would change the world"
                });

            _authenticationHandler = new AuthenticationHandler(_authenticationSettingsOptions, _userRepository);
        }

        [Fact]
        public void Authenticate_IncorrectCredentials_ShouldThrowAuthenticationException()
        {
            User returningUser = new()
            {
                Email = "test@test.se",
                Password = BCrypt.Net.BCrypt.HashPassword("This Is The Password")
            };

            _userRepository.GetByEmail(Arg.Any<string>()).Returns(returningUser);

            Func<Task> action = async () => await _authenticationHandler.Authenticate(new() { Email = returningUser.Email, Password = "Not the password" });

            action.Should().Throw<AuthenticationException>();
        }

        [Fact]
        public async Task Authenticate_CorrectCredentials_ShouldReturnAJwtToken()
        {
            User returningUser = new()
            {
                Email = "test@test.se",
                Password = BCrypt.Net.BCrypt.HashPassword("This Is The Password")
            };

            _userRepository.GetByEmail(Arg.Any<string>()).Returns(returningUser);

            var token= await _authenticationHandler.Authenticate(new AuthenticationRequest { Email = returningUser.Email, Password = "This Is The Password" });

            token.Should().NotBeEmpty();
        }
    }
}
