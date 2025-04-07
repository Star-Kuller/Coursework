using System.Data;
using Coursework.Interfaces.Database.Repositories;
using Coursework.Models.Entities;
using Dapper;

namespace Coursework.Database.Repositories;

public class SolutionRepository(IDbConnection connection, IDbTransaction transaction) : ISolutionRepository
{
    public async Task<long> AddAsync(Solution solution)
    {
        ArgumentNullException.ThrowIfNull(solution);

        const string sql = """
                           INSERT INTO solutions (s3_key, exercise_id, by_exercise_author)
                           VALUES (@S3Key, @ExerciseId, @ByExerciseAuthor)
                           RETURNING id
                           """;

        var result = await connection.QuerySingleOrDefaultAsync<long>(
            sql, solution, transaction);

        return result;
    }

    public async Task UpdateAsync(Solution solution)
    {
        ArgumentNullException.ThrowIfNull(solution);

        const string sql = """
                           UPDATE solutions
                           SET s3_key = @S3Key, exercise_id = @ExerciseId
                           WHERE id = @Id
                           """;

        await connection.ExecuteAsync(sql, solution, transaction);
    }

    public async Task DeleteAsync(long id)
    {
        const string sql = """
                           DELETE FROM solutions
                           WHERE id = @Id
                           """;

        await connection.ExecuteAsync(sql, new { Id = id }, transaction);
    }

    public async Task<Solution?> GetAsync(long id)
    {
        const string sql = """
                           SELECT s.*, 
                                  e.*
                           FROM solutions s
                           LEFT JOIN exercises e ON s.exercise_id = e.id
                           WHERE s.id = @Id
                           LIMIT 1
                           """;

        var result = await connection.QueryAsync<Solution, Exercise, Solution>(
            sql,
            (solution, exercise) =>
            {
                solution.Exercise = exercise;
                return solution;
            },
            new { Id = id },
            transaction,
            splitOn: "id"
        );

        return result.FirstOrDefault();
    }

    public async Task<List<Solution>> GetAllAsync()
    {
        const string sql = """
                           SELECT *
                           FROM solutions
                           ORDER BY id
                           """;

        var result = await connection.QueryAsync<Solution>(sql, transaction);
        return result.ToList();
    }

    public async Task<List<Solution>> GetAllByExerciseAsync(long exerciseId)
    {
        const string sql = """
                           SELECT *
                           FROM solutions
                           WHERE exercises_id = @ExerciseId
                           ORDER BY id
                           """;

        var result = await connection.QueryAsync<Solution>(sql, 
            new { ExerciseId = exerciseId }, transaction);
        return result.ToList();
    }
}