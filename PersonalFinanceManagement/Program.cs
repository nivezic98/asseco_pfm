using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PersonalFinanceManagement.API.Database;
using PersonalFinanceManagement.API.Database.Repositories;
using PersonalFinanceManagement.API.Services;
using PersonalFinanceManagement.API.Formatters;

namespace PersonalFinanceManagement.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddScoped<ITransactionService,TransactionService>();
        builder.Services.AddScoped<ITransactionRepository,TransactionRepository>();
        builder.Services.AddScoped<ICategoryService,CategoryService>();
        builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.Services.AddDbContext<TransactionDbContext>(options =>
            {
                 options.UseNpgsql(CreateConnectionString(builder.Configuration));
            });

        builder.Services.AddMvc(options =>
            {
                options.InputFormatters.Insert(0, new TransactionInput());
                options.InputFormatters.Insert(1, new CategoryInput());
            });

        builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        InitializeDatabase(app);

        app.MapControllers();

        app.Run();
    }
    
    private static string CreateConnectionString(IConfiguration configuration)
        {
            var username = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? configuration["Database:Username"];
            var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? configuration["Database:Password"];
            var database = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? configuration["Database:Name"];
            var host = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? configuration["Database:Host"];
            var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? configuration["Database:Port"];

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = int.Parse(port),
                Database = database,
                Username = username,
                Password = password,
                Pooling = true,
            };

            return builder.ConnectionString;
        }
        private static void InitializeDatabase(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope();

                scope.ServiceProvider.GetRequiredService<TransactionDbContext>().Database.Migrate();
            }
        }  
}
