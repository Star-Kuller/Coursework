using System.Data;
using Coursework.Interfaces.Database.Repositories;
using Coursework.Models.Entities;
using Dapper;

namespace Coursework.Database.Repositories;

public class HintRepository(IDbConnection connection, IDbTransaction transaction) 
    : IHintRepository
{
    public async Task<long> AddAsync(Hint hint)
    {
        ArgumentNullException.ThrowIfNull(hint);
        
        const string sql = """
                         INSERT INTO hints (exercise_id, cost, text)
                         VALUES (@ExerciseId, @Cost, @Text)
                         RETURNING id
                         """;

        var result = await connection.QuerySingleOrDefaultAsync<long>(
            sql, hint, transaction);
        return result;
    }

    public async Task UpdateAsync(Hint hint)
    {
        ArgumentNullException.ThrowIfNull(hint);
        
        const string sql = """
                         UPDATE hints
                         SET exercise_id = @ExerciseId, 
                             cost = @Cost, 
                             text = @Text
                         WHERE id = @Id
                         """;

        await connection.ExecuteAsync(sql, hint, transaction);
    }

    public async Task DeleteAsync(long id)
    {
        const string sql = """
                         DELETE FROM hints
                         WHERE id = @Id
                         """;

        await connection.ExecuteAsync(sql, new { Id = id }, transaction);
    }

    public async Task<Hint?> GetAsync(long id)
    {
        const string sql = """
                         SELECT *
                         FROM hints
                         WHERE id = @Id
                         LIMIT 1
                         """;

        var result = await connection.QuerySingleOrDefaultAsync<Hint?>(
            sql, new { Id = id }, transaction);
        return result;
    }

    public async Task<List<Hint>> GetAllAsync()
    {
        const string sql = """
                         SELECT *
                         FROM hints
                         ORDER BY id
                         """;

        var result = await connection.QueryAsync<Hint>(sql, transaction);
        return result.ToList();
    }
    
    public async Task<List<Hint>> GetAllByExerciseAsync(long exerciseId)
    {
        const string sql = """
                         SELECT *
                         FROM hints
                         WHERE exercise_id = @ExerciseId
                         ORDER BY cost
                         """;

        var result = await connection.QueryAsync<Hint>(
            sql, new { ExerciseId = exerciseId }, transaction);
        return result.ToList();
    }
}