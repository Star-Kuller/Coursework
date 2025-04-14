using Coursework.Models.Entities;

namespace Coursework.Interfaces.Database.Repositories;

public interface IDifficultyLevelRepository
{
    Task<List<DifficultyLevel>> GetAllAsync();
}