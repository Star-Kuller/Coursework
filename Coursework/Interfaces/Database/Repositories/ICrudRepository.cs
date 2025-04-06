namespace Coursework.Interfaces.Database.Repositories;

public interface ICrudRepository<T>
{
    Task<long> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(long id);
    Task<T?> GetAsync(long id);
    Task<List<T>> GetAllAsync();
}