using System.Data;
using Coursework.Interfaces.Database.Repositories;
using Coursework.Models;
using Coursework.Models.Entities;
using Dapper;

namespace Coursework.Database.Repositories;

public class ExerciseRepository(IDbConnection connection, IDbTransaction transaction)
    : IExerciseRepository
{
    public async Task<long> AddAsync(Exercise exercise)
    {
        ArgumentNullException.ThrowIfNull(exercise);
        
        const string insertExerciseSql = """
            INSERT INTO exercises (
                name, difficulty_id, score, short_description, full_description, 
                is_published, s3_key_source, s3_key_tests
            )
            VALUES (
                @Name, @DifficultyId, @Score, @ShortDescription, @FullDescription, 
                @IsPublished, @S3KeySource, @S3KeyTests
            )
            RETURNING id
            """;

        var exerciseId = await connection.QuerySingleOrDefaultAsync<long>(
            insertExerciseSql, exercise, transaction);

        if (exercise.Frameworks == null || !exercise.Frameworks.Any()) return exerciseId;
        
        const string insertFrameworksSql = """
                                           INSERT INTO frameworks_exercises (framework_id, exercise_id)
                                           VALUES (@FrameworkId, @ExerciseId)
                                           """;

        var frameworkParams = exercise.Frameworks
            .Where(f => f.Id > 0)
            .Select(f => new { FrameworkId = f.Id, ExerciseId = exerciseId });

        await connection.ExecuteAsync(insertFrameworksSql, frameworkParams, transaction);

        return exerciseId;
    }

    public async Task UpdateAsync(Exercise exercise)
    {
        ArgumentNullException.ThrowIfNull(exercise);
        
        const string updateExerciseSql = """
            UPDATE exercises
            SET 
                name = @Name, difficulty_id = @DifficultyId, score = @Score, 
                short_description = @ShortDescription, full_description = @FullDescription, 
                is_published = @IsPublished, s3_key_source = @S3KeySource, s3_key_tests = @S3KeyTests
            WHERE id = @Id
            """;

        await connection.ExecuteAsync(updateExerciseSql, exercise, transaction);
        
        const string deleteFrameworksSql = """
            DELETE FROM frameworks_exercises
            WHERE exercise_id = @ExerciseId
            """;

        await connection.ExecuteAsync(deleteFrameworksSql, new { ExerciseId = exercise.Id }, transaction);
        
        if (exercise.Frameworks != null && exercise.Frameworks.Any())
        {
            const string insertFrameworksSql = """
                INSERT INTO frameworks_exercises (framework_id, exercise_id)
                VALUES (@FrameworkId, @ExerciseId)
                """;

            var frameworkParams = exercise.Frameworks
                .Where(f => f.Id > 0)
                .Select(f => new { FrameworkId = f.Id, ExerciseId = exercise.Id });

            await connection.ExecuteAsync(insertFrameworksSql, frameworkParams, transaction);
        }
    }

    public async Task DeleteAsync(long id)
    {
        const string deleteExerciseSql = """
            DELETE FROM exercises
            WHERE id = @Id
            """;

        await connection.ExecuteAsync(deleteExerciseSql, new { Id = id }, transaction);
    }

    public async Task<Exercise?> GetAsync(long id)
    {
        const string sql = """
            SELECT e.*,
                   d.*,
                   s.*
            FROM exercises e
            LEFT JOIN difficulty_levels d ON e.difficulty_id = d.id
            LEFT JOIN (
                SELECT *
                FROM solutions
                WHERE by_exercise_author = TRUE
            ) s ON e.id = s.exercise_id
            WHERE e.id = @Id
            LIMIT 1
            """;

        const string frameworksSql = """
            SELECT f.*
            FROM frameworks f
            INNER JOIN frameworks_exercises fe ON f.id = fe.framework_id
            WHERE fe.exercise_id = @ExerciseId
            """;

        var exercise = await connection.QueryAsync<Exercise, DifficultyLevel, Solution, Exercise>(
            sql,
            (exercise, difficulty, solution) =>
            {
                exercise.Difficulty = difficulty;
                exercise.AuthorSolution = solution;
                return exercise;
            },
            new { Id = id },
            transaction,
            splitOn: "id"
        );

        var result = exercise.FirstOrDefault();
        if (result != null)
        {
            result.Frameworks = (await connection.QueryAsync<Framework>(
                frameworksSql,
                new { ExerciseId = id },
                transaction
            )).ToList();
        }

        return result;
    }

    public async Task<List<Exercise>> GetAllAsync()
    {
        return await GetAllAsync(null);
    }

    public async Task<List<Exercise>> GetAllAsync(string? search)
    {
        const string sql = """
                           SELECT e.*,
                                  d.*
                           FROM exercises e
                           LEFT JOIN difficulty_levels d ON e.difficulty_id = d.id
                           WHERE @SearchPattern IS NULL 
                              OR e.name LIKE @SearchPattern
                              OR e.short_description LIKE @SearchPattern
                           ORDER BY e.id
                           """;

        var exercises = await connection.QueryAsync<Exercise, DifficultyLevel, Exercise>(
            sql,
            (exercise, difficulty) =>
            {
                exercise.Difficulty = difficulty;
                return exercise;
            },
            new
            {
                SearchPattern = search != null ? $"%{search}%" : null
            },
            transaction: transaction,
            splitOn: "id"
        );

        return exercises.ToList();
    }
}