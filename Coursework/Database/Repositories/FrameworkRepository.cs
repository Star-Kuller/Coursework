using System.Data;
using Coursework.Interfaces.Database.Repositories;
using Coursework.Models;
using Dapper;

namespace Coursework.Database.Repositories;

public class FrameworkRepository(IDbConnection connection, IDbTransaction transaction)
    : IFrameworkRepository
{
    public async Task<long> AddAsync(Framework language)
    {
        ArgumentNullException.ThrowIfNull(language);

        const string sql = """
                           INSERT INTO frameworks (name, description, language_id)
                           VALUES (@Name, @Description, @LanguageId)
                           RETURNING id
                           """;

        var result = await connection.QuerySingleOrDefaultAsync<long>(
            sql, language, transaction);

        return result;
    }

    public async Task UpdateAsync(Framework language)
    {
        ArgumentNullException.ThrowIfNull(language);

        const string sql = """
                           UPDATE frameworks
                           SET name = @Name, description = @Description, language_id = @LanguageId
                           WHERE id = @Id
                           """;

        await connection.ExecuteAsync(sql, language, transaction);
    }

    public async Task DeleteAsync(long id)
    {
        const string sql = """
                           DELETE FROM frameworks
                           WHERE id = @Id
                           """;

        await connection.ExecuteAsync(sql, new { Id = id }, transaction);
    }

    public async Task<Framework?> GetAsync(long id)
    {
        const string sql = """
                           SELECT *
                           FROM frameworks
                           WHERE id = @Id
                           LIMIT 1
                           """;

        var result = await connection.QuerySingleOrDefaultAsync<Framework?>(
            sql, new { Id = id }, transaction);

        return result;
    }

    public async Task<List<Framework>> GetAllAsync()
    {
        const string sql = """
                           SELECT *
                           FROM frameworks
                           ORDER BY id
                           """;

        var result = await connection.QueryAsync<Framework>(sql, transaction);
        return result.ToList();
    }

    public async Task<List<Framework>> GetAllWithLanguageAsync()
    {
        const string sql = """
                           SELECT 
                               f.*, 
                               pl.*
                           FROM frameworks f
                           LEFT JOIN programing_languages pl ON f.language_id = pl.id
                           ORDER BY f.id
                           """;

        var result = await connection.QueryAsync<Framework, ProgrammingLanguage, Framework>(
            sql,
            (framework, language) =>
            {
                framework.Language = language;
                return framework;
            },
            splitOn: "id",
            transaction: transaction
        );

        return result.ToList();
    }
}