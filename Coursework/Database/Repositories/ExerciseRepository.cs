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
                name, difficulty_id, language_id, score, short_description, full_description, 
                is_published, s3_key_source, s3_key_tests
            )
            VALUES (
                @Name, @DifficultyId, @LanguageId, @Score, @ShortDescription, @FullDescription, 
                @IsPublished, @S3KeySource, @S3KeyTests
            )
            RETURNING id
            """;

        var exerciseId = await connection.QuerySingleOrDefaultAsync<long>(
            insertExerciseSql, exercise, transaction);

        if (!exercise.Frameworks.Any()) return exerciseId;
        
        const string insertFrameworksSql = """
                                           INSERT INTO frameworks_exercises (framework_id, exercise_id)
                                           VALUES (@FrameworkId, @ExerciseId)
                                           """;

        var frameworkParams = exercise.Frameworks
            .Where(f => f.Id > 0)
            .Select(f => new { FrameworkId = f.Id, ExerciseId = exerciseId });

        await connection.ExecuteAsync(insertFrameworksSql, frameworkParams, transaction);
        
        if (!exercise.Hints.Any()) return exerciseId;
        
        foreach (var hint in exercise.Hints)
            hint.ExerciseId = exerciseId;
        
        const string insertHintsSql = """
                                           INSERT INTO hints (exercise_id, cost, text)
                                           VALUES (@ExerciseId, @Cost, @Text)
                                           """;

        await connection.ExecuteAsync(insertHintsSql, exercise.Hints, transaction);

        return exerciseId;
    }

    public async Task UpdateAsync(Exercise exercise)
    {
        ArgumentNullException.ThrowIfNull(exercise);
        
        const string updateExerciseSql = """
            UPDATE exercises
            SET 
                name = @Name, difficulty_id = @DifficultyId, language_id = @LanguageId, score = @Score, 
                short_description = @ShortDescription, full_description = @FullDescription, 
                is_published = @IsPublished, s3_key_source = @S3KeySource, s3_key_tests = @S3KeyTests
            WHERE id = @Id
            """;

        await connection.ExecuteAsync(updateExerciseSql, exercise, transaction);
        
        const string deleteFrameworksSql = """
            DELETE FROM frameworks_exercises
            WHERE exercise_id = @ExerciseId
            """;
        
        const string deleteHintsSql = """
            DELETE FROM hints
            WHERE exercise_id = @ExerciseId
            """;

        await connection.ExecuteAsync(deleteFrameworksSql, new { ExerciseId = exercise.Id }, transaction);
        await connection.ExecuteAsync(deleteHintsSql, new { ExerciseId = exercise.Id }, transaction);

        
        if (exercise.Frameworks.Any())
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
        
        if (exercise.Hints.Any())
        {
            foreach (var hint in exercise.Hints)
                hint.ExerciseId = exercise.Id;
        
            const string insertHintsSql = """
                                          INSERT INTO hints (exercise_id, cost, text)
                                          VALUES (@ExerciseId, @Cost, @Text)
                                          """;

            await connection.ExecuteAsync(insertHintsSql, exercise.Hints, transaction);
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
                                  l.*,
                                  s.*
                           FROM exercises e
                           LEFT JOIN difficulty_levels d ON e.difficulty_id = d.id
                           LEFT JOIN programing_languages l ON e.language_id = l.id
                           LEFT JOIN solutions s ON e.id = s.exercise_id
                           WHERE e.id = @Id
                           """;
    
        const string frameworksSql = """
                                     SELECT f.*
                                     FROM frameworks f
                                     INNER JOIN frameworks_exercises fe ON f.id = fe.framework_id
                                     WHERE fe.exercise_id = @ExerciseId
                                     """;
    
        const string hintsSql = """
                                     SELECT h.*
                                     FROM hints h
                                     WHERE h.exercise_id = @ExerciseId
                                     ORDER BY h.cost
                                     """;
    
        var exercise = await connection.QueryAsync<Exercise, DifficultyLevel, ProgrammingLanguage, Solution, Exercise>(
            sql,
            (exercise, difficulty, language, solution) =>
            {
                exercise.Difficulty = difficulty;
                exercise.Language = language;
                exercise.Solutions.Add(solution);
                return exercise;
            },
            new { Id = id },
            transaction,
            splitOn: "id"
        );
    
        var result = exercise.GroupBy(e => e.Id).Select(g =>
        {
            var ex = g.First();
            ex.AuthorSolution = g
                .SelectMany(e => e.Solutions)
                .FirstOrDefault(s => s.AuthorId == ex.AuthorId);
            ex.Solutions = g
                .SelectMany(e => e.Solutions)
                .Where(s => s.AuthorId != ex.AuthorId)
                .ToList();
            return ex;
        }).FirstOrDefault();

        if (result == null) return result;
        
        result.Frameworks = (await connection.QueryAsync<Framework>(
            frameworksSql,
            new { ExerciseId = id },
            transaction
        )).ToList();
    
        result.Hints = (await connection.QueryAsync<Hint>(
            hintsSql,
            new { ExerciseId = id },
            transaction
        )).ToList();

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
                                  d.*,
                                  l.*
                           FROM exercises e
                           LEFT JOIN difficulty_levels d ON e.difficulty_id = d.id
                           LEFT JOIN programing_languages l ON e.language_id = l.id
                           WHERE @SearchPattern IS NULL 
                              OR e.name LIKE @SearchPattern
                              OR e.short_description LIKE @SearchPattern
                           ORDER BY e.id
                           """;

        var exercises = await connection.QueryAsync<Exercise, DifficultyLevel, ProgrammingLanguage, Exercise>(
            sql,
            (exercise, difficulty, language) =>
            {
                exercise.Difficulty = difficulty;
                exercise.Language = language;
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