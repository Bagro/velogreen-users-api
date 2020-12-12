using System.Data;
using System.Threading.Tasks;
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
    }
}
