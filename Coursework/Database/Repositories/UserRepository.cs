using System.Data;
using Coursework.Interfaces.Database.Repositories;
using Coursework.Models.Entities;
using Dapper;

namespace Coursework.Database.Repositories;

public class UserRepository(IDbConnection connection, IDbTransaction transaction)
    : IUserRepository
{
    public async Task<long> AddAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        const string insertUserSql = """
            INSERT INTO users (
                name, email, password_hash, about, score, role_id
            )
            VALUES (
                @Name, @Email, @PasswordHash, @About, @Score, @RoleId
            )
            RETURNING id
            """;

        var userId = await connection.QuerySingleOrDefaultAsync<long>(
            insertUserSql, user, transaction);

        return userId;
    }

    public async Task UpdateAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        const string updateUserSql = """
            UPDATE users
            SET 
                name = @Name, 
                email = @Email, 
                password_hash = @PasswordHash, 
                about = @About, 
                score = @Score, 
                role_id = @RoleId
            WHERE id = @Id
            """;

        await connection.ExecuteAsync(updateUserSql, user, transaction);
    }

    public async Task DeleteAsync(long id)
    {
        const string deleteUserSql = """
            DELETE FROM users
            WHERE id = @Id
            """;

        await connection.ExecuteAsync(deleteUserSql, new { Id = id }, transaction);
    }

    public async Task<User?> GetAsync(long id)
    {
        const string sql = """
                           SELECT u.*,
                                  r.*
                           FROM users u
                           LEFT JOIN roles r ON u.role_id = r.id
                           WHERE u.id = @Id
                           """;
    
        const string exercisesSql = """
                                    SELECT e.*,
                                           d.*,
                                           l.*
                                    FROM exercises e
                                    LEFT JOIN difficulty_levels d ON e.difficulty_id = d.id
                                    LEFT JOIN programing_languages l ON e.language_id = l.id
                                    WHERE e.author_id = @UserId
                                    """;
    
        const string solutionsSql = """
                                    SELECT s.*,
                                           e.*
                                    FROM solutions s
                                    LEFT JOIN exercises e ON s.exercise_id = e.id
                                    WHERE s.author_id = @UserId
                                    """;
    
        var userResult = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Id = id },
            transaction,
            splitOn: "id"
        );
    
        var user = userResult.FirstOrDefault();
        if (user == null) return null;
        
        var exercises = await connection.QueryAsync<Exercise, DifficultyLevel, ProgrammingLanguage, Exercise>(
            exercisesSql,
            (exercise, difficulty, language) =>
            {
                exercise.Difficulty = difficulty;
                exercise.Language = language;
                return exercise;
            },
            new { UserId = id },
            transaction,
            splitOn: "id"
        );
        
        user.Exercises = exercises.ToList();
        
        var solutions = await connection.QueryAsync<Solution, Exercise, Solution>(
            solutionsSql,
            (solution, exercise) =>
            {
                solution.Exercise = exercise;
                return solution;
            },
            new { UserId = id },
            transaction,
            splitOn: "id"
        );
        
        user.Solutions = solutions.ToList();

        return user;
    }

    public async Task<List<User>> GetAllAsync()
    {
        const string sql = """
                           SELECT u.*,
                                  r.*
                           FROM users u
                           LEFT JOIN roles r ON u.role_id = r.id
                           ORDER BY u.id
                           """;

        var users = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            transaction: transaction,
            splitOn: "id"
        );

        return users.ToList();
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = """
                           SELECT u.*,
                                  r.*
                           FROM users u
                           LEFT JOIN roles r ON u.role_id = r.id
                           WHERE u.email = @Email
                           """;
    
        var userResult = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Email = email },
            transaction,
            splitOn: "id"
        );
    
        var user = userResult.FirstOrDefault();
        if (user == null) return null;
        
        const string exercisesSql = """
                                    SELECT e.*,
                                           d.*,
                                           l.*
                                    FROM exercises e
                                    LEFT JOIN difficulty_levels d ON e.difficulty_id = d.id
                                    LEFT JOIN programing_languages l ON e.language_id = l.id
                                    WHERE e.author_id = @UserId
                                    """;
        
        var exercises = await connection.QueryAsync<Exercise, DifficultyLevel, ProgrammingLanguage, Exercise>(
            exercisesSql,
            (exercise, difficulty, language) =>
            {
                exercise.Difficulty = difficulty;
                exercise.Language = language;
                return exercise;
            },
            new { UserId = user.Id },
            transaction,
            splitOn: "id"
        );
        
        user.Exercises = exercises.ToList();
        
        const string solutionsSql = """
                                    SELECT s.*,
                                           e.*
                                    FROM solutions s
                                    LEFT JOIN exercises e ON s.exercise_id = e.id
                                    WHERE s.author_id = @UserId
                                    """;
        
        var solutions = await connection.QueryAsync<Solution, Exercise, Solution>(
            solutionsSql,
            (solution, exercise) =>
            {
                solution.Exercise = exercise;
                return solution;
            },
            new { UserId = user.Id },
            transaction,
            splitOn: "id"
        );
        
        user.Solutions = solutions.ToList();

        return user;
    }
}