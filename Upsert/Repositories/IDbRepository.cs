namespace Upsert.Repositories
{
    public interface IDbRepository
    {
        Task ExecuteSqlRawAsync<T>(string query) where T : class;
    }
}
