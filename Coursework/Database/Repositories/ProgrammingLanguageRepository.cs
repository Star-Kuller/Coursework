using System.Data;
using Coursework.Interfaces.Database.Repositories;
using Coursework.Models;
using Coursework.Models.Entities;
using Dapper;

namespace Coursework.Database.Repositories;

public class ProgrammingLanguageRepository(IDbConnection connection, IDbTransaction transaction)
    : IProgrammingLanguageRepository
{
    public async Task<long> AddAsync(ProgrammingLanguage language)
    {
        ArgumentNullException.ThrowIfNull(language);

        const string sql = """
                           INSERT INTO programing_languages (name, description)
                           VALUES (@Name, @Description)
                           RETURNING id
                           """;

        var result = await connection.QuerySingleOrDefaultAsync<long>(
            sql, language, transaction);

        return result;
    }

    public async Task UpdateAsync(ProgrammingLanguage language)
    {
        ArgumentNullException.ThrowIfNull(language);

        const string sql = """
                           UPDATE programing_languages
                           SET name = @Name, description = @Description
                           WHERE id = @Id
                           """;

        await connection.ExecuteAsync(sql, language, transaction);
    }

    public async Task DeleteAsync(long id)
    {
        const string sql = """
                           DELETE FROM programing_languages
                           WHERE id = @Id
                           """;

        await connection.ExecuteAsync(sql, new { Id = id }, transaction);
    }

    public async Task<ProgrammingLanguage?> GetAsync(long id)
    {
        const string sql = """
                           SELECT id, name, description
                           FROM programing_languages
                           WHERE id = @Id
                           LIMIT 1
                           """;

        var result = await connection.QuerySingleOrDefaultAsync<ProgrammingLanguage?>(
            sql, new { Id = id }, transaction);

        return result;
    }

    public async Task<List<ProgrammingLanguage>> GetAllAsync()
    {
        const string sql = """
                           SELECT id, name, description
                           FROM programing_languages
                           ORDER BY id
                           """;

        var result = await connection.QueryAsync<ProgrammingLanguage>(sql, transaction);
        return result.ToList();
    }
}