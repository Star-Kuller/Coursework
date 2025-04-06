using Coursework.Models.Entities;

namespace Coursework.Interfaces.Database.Repositories;

public interface ISolutionRepository : ICrudRepository<Solution>
{
    Task<List<Solution>> GetAllByExerciseAsync(long exerciseId);
}