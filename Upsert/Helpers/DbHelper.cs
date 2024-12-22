using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using Upsert.Constant;

namespace Upsert.Helpers
{
    public static class DbHelper
    {
        public static List<(string ColumnName, string InsertValues)> Class2Column<TClass, TRequest>(
             TClass databaseClass,
             TRequest request)
        {
            var result = new List<(string ColumnName, string InsertValues)>();

            PropertyInfo[] classProperties = typeof(TClass).GetProperties();

            Dictionary<string, string> validProperties = typeof(TRequest).GetProperties()
            .ToDictionary(
                p => p.Name,
                p =>
                {
                    object value = p.GetValue(request);
                    return value is DateTime dateTime
                        ? dateTime.ToString(ConstantValues.DateFormat.yyyy_MM_dd_HH_mm_ss) // postgrsql date format
                        : value?.ToString() ?? string.Empty;
                });

            foreach (PropertyInfo classProperty in classProperties)
            {
                if (!IsDbType(classProperty.PropertyType))
                    continue;

                string columnName = classProperty.Name;

                if (validProperties.TryGetValue(columnName, out string insertValue))
                {
                    result.Add((columnName, insertValue));
                }
            }
            return result;
        }

        public static (string Tableame, string PrimaryKey) GetTableName<T>(IModel model, T target)
        {
            IEnumerable<IEntityType> entityTypes = model.GetEntityTypes();
            IEntityType entityType = entityTypes.First(t => t.ClrType == typeof(T));
            IAnnotation tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
            string primaryKeyName = entityType.FindPrimaryKey().Properties.First().Name;
            string tableName = tableNameAnnotation.Value.ToString();

            return (tableName, primaryKeyName);
        }

        private static bool IsDbType(Type type)
        {
            return
                type.IsValueType ||
                type.IsPrimitive ||
                new Type[]
                {
                    typeof(string),
                    typeof(decimal),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid)
                }.Contains(type);
        }
    }
}
