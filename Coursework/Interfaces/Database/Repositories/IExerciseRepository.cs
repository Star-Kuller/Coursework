using Coursework.Models.Entities;

namespace Coursework.Interfaces.Database.Repositories;

public interface IExerciseRepository : ICrudRepository<Exercise>
{
    Task<List<Exercise>> GetAllAsync(string search);
    Task AddLikeAsync(long exerciseId, long userId);
    Task RemoveLikeAsync(long exerciseId, long userId);
    Task OpenHintAsync(long hintId, long userId);
}