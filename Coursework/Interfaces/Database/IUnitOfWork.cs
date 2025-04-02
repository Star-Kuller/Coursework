namespace Coursework.Interfaces.Database;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    
    Task CommitAsync(CancellationToken cancellationToken);
    Task RollBackAsync(CancellationToken cancellationToken);
}