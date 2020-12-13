using System;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using VeloGreen.Users.Api.Entities;
using VeloGreen.Users.Api.Exceptions;
using VeloGreen.Users.Api.Handlers;
using VeloGreen.Users.Api.Storage;
using Xunit;

namespace VeloGreen.Users.Api.Tests.Handlers
{
    public class UserHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserHandler _userHandler;

        public UserHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

            _userHandler = new UserHandler(_userRepository, _httpContextAccessor);
        }

        [Fact]
        public void Register_EmailAlreadyUsed_ShouldThrowDuplicateNameException()
        {
            _userRepository.IsEmailUsed(Arg.Any<string>()).Returns(true);

            Func<Task> action = async () => await _userHandler.Register(new RegisterRequest());

            action.Should().Throw<DuplicateNameException>();
        }

        [Fact]
        public async Task Register_ValidRequest_ShouldSendCorrectUserToRepository()
        {
            RegisterRequest registerRequest = new()
            {
                Email = "test@test.com",
                FirstName = "Bert",
                LastName = "Lark",
                Password = "Top Secret",
            };

            User expectedUser = new()
            {
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
            };

            User user = null;

            await _userRepository.Save(Arg.Do<User>(x => user = x));
            await _userHandler.Register(registerRequest);

            user.Email.Should().Be(expectedUser.Email);
            user.FirstName.Should().Be(expectedUser.FirstName);
            user.LastName.Should().Be(expectedUser.LastName);
            user.Password.Should().StartWith("$2a$11$");
        }

        [Fact]
        public void Update_UserDoesNotExist_ShouldThrowUserNotFoundException()
        {
            _userRepository.GetById(Arg.Any<Guid>()).Returns((User)null);

            Func<Task> action = async () => await _userHandler.Update(new UpdateUserRequest());

            action.Should().Throw<UserNotFoundException>();
        }

        [Fact]
        public async Task Update_ChangeLastNameNoPasswordChange_ShouldSendCorrectUserToRepository()
        {
            User storedUser = new()
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                FirstName = "Bert",
                LastName = "Lark",
                Password = BCrypt.Net.BCrypt.HashPassword("Top Secret"),
            };

            _userRepository.GetById(Arg.Any<Guid>()).Returns(storedUser);
            
                
        }
    }
}
