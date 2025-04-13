using Coursework.Models.Entities;

namespace Coursework.Interfaces.Database.Repositories;

public interface ISolutionRepository : ICrudRepository<Solution>
{
    Task<List<Solution>> GetAllByExerciseAsync(long exerciseId);
    Task AddLikeAsync(long solutionId, long userId);
    Task RemoveLikeAsync(long solutionId, long userId);
}