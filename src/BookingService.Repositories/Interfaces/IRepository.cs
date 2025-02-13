namespace BookingService.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();

    Task<T?> GetByIdAsync(string id);

    Task SaveAsync(T entity);

    Task DeleteAsync(T entity);

    Task UpdateAsync(T entity);
}
