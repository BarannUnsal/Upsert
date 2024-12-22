using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Upsert.Context;
using Upsert.Entities;
using Upsert.Helpers;
using Upsert.Repositories;
using Upsert.Services;

IConfigurationBuilder builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfigurationRoot configuration = builder.Build();


var services = new ServiceCollection()
    .AddDbContext<AppDbContext>(opt =>
    {
        opt.UseNpgsql(configuration.GetConnectionString("Default"));
    })
    .AddTransient<IDbRepository, DbRepository>()
    .AddTransient<IUpsertService, UpsertService>()
    .BuildServiceProvider();

IServiceScope scope = services.CreateScope();
AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.Migrate();

var requestedUser = new User
{
    Id = 1,
    Name = "test user"
};

await Upsert(requestedUser);

async Task Upsert<T>(T table) where T : class
{
    List<(string ColumnName, string InsertValues)> class2InsertColumn = DbHelper.Class2Column(table, table);


    (string TableName, string PrimaryKey) = DbHelper.GetTableName(dbContext.Model, table);

    IUpsertService upsertService = scope.ServiceProvider.GetRequiredService<IUpsertService>();

    await upsertService.Upsert<T>(
         TableName,
         class2InsertColumn,
         PrimaryKey,
         class2InsertColumn);
}