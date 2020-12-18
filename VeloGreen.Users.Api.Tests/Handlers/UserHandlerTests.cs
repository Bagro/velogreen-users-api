using System;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using VeloGreen.Users.Api.Entities;
using VeloGreen.Users.Api.Exceptions;
using VeloGreen.Users.Api.Handlers;
using VeloGreen.Users.Api.Storage;
using VeloGreen.Users.Api.Verifiers;
using Xunit;

namespace VeloGreen.Users.Api.Tests.Handlers
{
    public class UserHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccessVerifier _accessVerifier;
        private readonly UserHandler _userHandler;

        public UserHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _accessVerifier = Substitute.For<IAccessVerifier>();

            _userHandler = new UserHandler(_userRepository, _accessVerifier);
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
            _accessVerifier.HaveAccess(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            
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
            _accessVerifier.HaveAccess(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            UpdateUserRequest updateUserRequest = new()
            {
                Id = storedUser.Id,
                FirstName = storedUser.FirstName,
                LastName = "Blark"
            };

            User expectedUser = new()
            {
                Id = storedUser.Id,
                Email = storedUser.Email,
                FirstName = storedUser.FirstName,
                LastName = updateUserRequest.LastName,
                Password = storedUser.Password
            };

            User savedUser = null;
            await _userRepository.Save(Arg.Do<User>(x => savedUser = x));

            await _userHandler.Update(updateUserRequest);
            
            savedUser.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task Update_ChangePassword_ShouldDifferentPasswordToRepository()
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
            _accessVerifier.HaveAccess(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            UpdateUserRequest updateUserRequest = new()
            {
                Id = storedUser.Id,
                FirstName = storedUser.FirstName,
                LastName = storedUser.LastName,
                CurrentPassword = "Top Secret",
                NewPassword = "Secret"
            };

            var password = storedUser.Password;
            
            User savedUser = null;
            await _userRepository.Save(Arg.Do<User>(x => savedUser = x));

            await _userHandler.Update(updateUserRequest);

            savedUser.Password.Should().NotBe(password).And.StartWith("$2a$11$");
        }

        [Fact]
        public void Update_NoAccess_ShouldThrowUnauthorizedAccessException()
        {
            _accessVerifier.HaveAccess(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            UpdateUserRequest updateUserRequest = new();

            Func<Task> action = async () => await _userHandler.Update(updateUserRequest);

            action.Should().Throw<UnauthorizedAccessException>();
        }

        [Fact]
        public void GetUserById_NoAccess_ShouldThrowUnauthorizedAccessException()
        {
            _accessVerifier.HaveAccess(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
            
            Func<Task> action = async () => await _userHandler.GetUserById(Guid.NewGuid());

            action.Should().Throw<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task GetUserById_FoundUser_ShouldReturnCorrectUser()
        {
            User storedUser = new()
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                FirstName = "Bert",
                LastName = "Lark",
                Password = BCrypt.Net.BCrypt.HashPassword("Top Secret"),
            };

            User expectedUser = new()
            {
                Id = storedUser.Id,
                Email = storedUser.Email,
                FirstName = storedUser.FirstName,
                LastName = storedUser.LastName,
                Password = string.Empty
            };
            
            _userRepository.GetById(Arg.Any<Guid>()).Returns(storedUser);
            _accessVerifier.HaveAccess(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            var user = await _userHandler.GetUserById(storedUser.Id);
            
            user.Should().BeEquivalentTo(expectedUser);
        }
    }
}
