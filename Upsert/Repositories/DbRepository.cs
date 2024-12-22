using Microsoft.EntityFrameworkCore;
using Upsert.Context;

namespace Upsert.Repositories
{
    public class DbRepository : IDbRepository
    {
        private readonly AppDbContext _appDbContext;

        public DbRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task ExecuteSqlRawAsync<T>(string query) where T : class
        {
            await _appDbContext.Database.ExecuteSqlRawAsync(query);
        }
    }
}
