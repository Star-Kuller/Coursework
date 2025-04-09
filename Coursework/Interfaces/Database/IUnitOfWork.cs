using Coursework.Interfaces.Database.Repositories;

namespace Coursework.Interfaces.Database;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IProgrammingLanguageRepository Languages { get; }
    IFrameworkRepository Frameworks { get; }
    IDifficultyLevelRepository DifficultyLevels { get; }
    IExerciseRepository Exercises { get; }
    ISolutionRepository Solutions { get; }
    IHintRepository Hints { get; }
    
    Task CommitAsync(CancellationToken cancellationToken);
    Task RollBackAsync(CancellationToken cancellationToken);
}