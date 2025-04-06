using System.Data;
using Coursework.Interfaces.Database.Repositories;
using Coursework.Models;
using Coursework.Models.Entities;
using Dapper;

namespace Coursework.Database.Repositories;

public class DifficultyLevelRepository(IDbConnection connection, IDbTransaction transaction) : IDifficultyLevelRepository
{
    public async Task<List<DifficultyLevel>> GetAllAsync()
    {
        const string sql = """
                           SELECT id, name
                           FROM difficulty_levels
                           ORDER BY id
                           """;

        var result = await connection.QueryAsync<DifficultyLevel>(sql, transaction);
        return result.ToList();
    }
}