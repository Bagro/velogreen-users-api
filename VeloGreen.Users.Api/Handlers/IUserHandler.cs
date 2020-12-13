using System.Threading.Tasks;
using VeloGreen.Users.Api.Entities;

namespace VeloGreen.Users.Api.Handlers
{
    public interface IUserHandler
    {
        Task Register(RegisterRequest registerRequest);

        Task Update(UpdateUserRequest updateUserRequest);
        
        Task<string> Authenticate(AuthenticationRequest authenticationRequest);
        
        Task<User> GetUserByEmail(string email);
    }
}
