using Coursework.Models.Entities;

namespace Coursework.Interfaces.Database.Repositories;

public interface IHintRepository : ICrudRepository<Hint>
{
    Task<List<Hint>> GetAllByExerciseAsync(long exerciseId);
}