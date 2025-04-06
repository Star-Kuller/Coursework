using Coursework.Models;
using Coursework.Models.Entities;

namespace Coursework.Interfaces.Database.Repositories;

public interface IExerciseRepository : ICrudRepository<Exercise>
{
    Task<List<Exercise>> GetAllAsync(string search);
}