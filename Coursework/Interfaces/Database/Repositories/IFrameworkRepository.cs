using Coursework.Models.Entities;

namespace Coursework.Interfaces.Database.Repositories;

public interface IFrameworkRepository : ICrudRepository<Framework>
{
    Task<List<Framework>> GetAllWithLanguageAsync();
}