using System.Threading.Tasks;
using VeloGreen.Users.Api.Entities;

namespace VeloGreen.Users.Api.Handlers
{
    public interface IAuthenticationHandler
    {
        Task<string> Authenticate(AuthenticationRequest authenticationRequest);
    }
}
