using Coursework.Models;

namespace Coursework.Interfaces.Database.Repositories;

public interface IDifficultyLevelRepository
{
    Task<List<DifficultyLevel>> GetAllAsync();
}