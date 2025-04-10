using Coursework.Models.Entities;

namespace Coursework.Interfaces.Database.Repositories;

public interface IUserRepository : ICrudRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}