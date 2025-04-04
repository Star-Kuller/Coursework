using Coursework.Models;

namespace Coursework.Interfaces.Database.Repositories;

public interface IExerciseRepository : ICrudRepository<Exercise>
{
    Task<List<Exercise>> GetAllAsync(string search);
}