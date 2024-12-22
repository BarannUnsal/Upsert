namespace Upsert.Services
{
    public interface IUpsertService
    {
        Task Upsert<T>(
            string tableName,
            ICollection<(string ColumnName, string InsertValues)> entities,
            string joinColumns,
            ICollection<(string ColumnName, string Value)>? updatedEntities) where T : class;
    }
}
