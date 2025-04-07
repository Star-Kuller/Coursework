using Coursework.Models.Entities;

namespace Coursework.Interfaces.Database.Repositories;

public interface IExerciseRepository : ICrudRepository<Exercise>
{
    Task<Exercise?> GetWithSolutionsAsync(long id);
    Task<List<Exercise>> GetAllAsync(string search);
}