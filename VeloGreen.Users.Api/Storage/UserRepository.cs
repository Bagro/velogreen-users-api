using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VeloGreen.Users.Api.Entities;

namespace VeloGreen.Users.Api.Storage
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _applicationDbContext = dbContext;
        }
        
        public async Task Save(User user)
        {
            if (user.Id == Guid.Empty)
            {   
                user.Id = Guid.NewGuid();

                _applicationDbContext.Users.Add(user);
            }
            else
            {
                var storedUser = await _applicationDbContext.Users.SingleOrDefaultAsync(x => x.Id == user.Id);

                storedUser.FirstName = user.FirstName;
                storedUser.LastName = user.LastName;

                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    storedUser.Password = user.Password;
                }
            }

            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _applicationDbContext.Users.SingleOrDefaultAsync(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
        }

        public Task<User> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsEmailUsed(string email)
        {
            return await _applicationDbContext.Users.AnyAsync(x => x.Email.Equals(email));
        }
    }
}
