using System.Text;
using Upsert.Repositories;

namespace Upsert.Services
{
    public class UpsertService : IUpsertService
    {
        private readonly IDbRepository _dbRepository;

        public UpsertService(IDbRepository dbRepository)
        {
            _dbRepository = dbRepository;
        }

        public async Task Upsert<T>(
            string tableName,
            ICollection<(string ColumnName, string InsertValues)> entities,
            string joinColumns,
            ICollection<(string ColumnName, string Value)>? updatedEntities) where T : class
        {
            if (tableName is null || entities is null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var query = new StringBuilder();
            query.Append($"INSERT INTO {EscapeName(tableName)} (");
            query.Append(string.Join(", ", entities.Select(e => EscapeName(e.ColumnName))));
            query.Append(") VALUES (");
            query.Append(string.Join(", ", entities.Select(ec => EscapeValue(ec.InsertValues))));
            query.Append(") ON CONFLICT (");
            query.Append(string.Join(", ", EscapeName(joinColumns)));
            query.Append(") ");
            query.Append("DO UPDATE SET ");
            query.Append(string.Join(", ", updatedEntities.Select(e => $"{EscapeName(e.ColumnName)} = {EscapeValue(e.Value)}")));

            await _dbRepository.ExecuteSqlRawAsync<T>(query.ToString() + ";");
        }

        private static string EscapeName(string name) => "\"" + name + "\"";

        private static string EscapeValue(string name) => "\'" + name + "\'";
    }
}
