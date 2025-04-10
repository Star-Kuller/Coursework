using Coursework.Models.Entities;

namespace Coursework.Interfaces.Database.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetAsync(long id);
}