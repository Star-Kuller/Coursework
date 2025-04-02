namespace Coursework.Interfaces.Database;

public interface IUnitOfWorkFactory
{
    Task<IUnitOfWork> CreateAsync(CancellationToken token);
}