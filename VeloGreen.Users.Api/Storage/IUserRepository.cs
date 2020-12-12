using System;
using System.Threading.Tasks;
using VeloGreen.Users.Api.Entities;

namespace VeloGreen.Users.Api.Storage
{
    public interface IUserRepository
    {
        Task Save(User user);

        Task<User> GetByEmail(string email);

        Task<User> GetById(Guid id);

        Task<bool> IsEmailUsed(string email);
    }
}
