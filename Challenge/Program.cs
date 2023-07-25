using Challenge.LocalDbContext;
using Challenge.Managers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string localAppData =
    Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData);
string localDbPath
    = Path.Combine(localAppData, "Challenge");

if (!Directory.Exists(localDbPath))
    Directory.CreateDirectory(localDbPath);

var connection = new SqliteConnectionStringBuilder()
{
    DataSource = Path.Combine(localDbPath, "local.db")
};
builder.Services.Configure<SQLiteDbContextOptions<SQLiteDbContext>>(config =>
{
    config.DbContextOptions.UseSqlite(connection.ToString());
});
builder.Services.AddDbContext<SQLiteDbContext>(config =>
        config.UseSqlite(connection.ToString()),
    ServiceLifetime.Transient,
    ServiceLifetime.Transient
);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<RandomDataManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
