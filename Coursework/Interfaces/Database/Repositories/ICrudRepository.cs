namespace Coursework.Interfaces.Database.Repositories;

public interface ICrudRepository<T>
{
    Task<long> AddAsync(T user);
    Task UpdateAsync(T user);
    Task DeleteAsync(long id);
    Task<T?> GetAsync(long id);
    Task<List<T>> GetAllAsync();
}