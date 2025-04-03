using Coursework.Database.Repositories;
using Coursework.Interfaces.Database;
using Coursework.Interfaces.Database.Repositories;
using Npgsql;

namespace Coursework.Database.Infrastructure;

public class UnitOfWork(NpgsqlConnection connection, NpgsqlTransaction transaction) : IUnitOfWork
{
    private bool _commited;

    public IProgrammingLanguageRepository Languages => new ProgrammingLanguageRepository(connection, transaction);
    public IFrameworkRepository Frameworks => new FrameworkRepository(connection, transaction);

    public async Task CommitAsync(CancellationToken token)
    {
        if (_commited)
        {
            throw new InvalidOperationException("Already committed");
        }
        _commited = true;
        await transaction.CommitAsync(token);
    }

    public async Task RollBackAsync(CancellationToken token)
    {
        await transaction.RollbackAsync(token);
        transaction = await connection.BeginTransactionAsync(token);
    }
    
    public async ValueTask DisposeAsync()
    {
        await connection.DisposeAsync();
        await transaction.DisposeAsync();
    }

    public void Dispose()
    {
        connection.Dispose();
        transaction.Dispose();
    }
}