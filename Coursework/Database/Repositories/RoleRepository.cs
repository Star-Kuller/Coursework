using System.Data;
using Coursework.Interfaces.Database.Repositories;
using Coursework.Models.Entities;
using Dapper;

namespace Coursework.Database.Repositories;

public class RoleRepository(IDbConnection connection, IDbTransaction transaction) : IRoleRepository
{
    public async Task<Role?> GetAsync(long id)
    {
        const string sql = """
                           SELECT id, name
                           FROM roles
                           WHERE id = @Id
                           """;

        return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Id = id }, transaction);
    }
}