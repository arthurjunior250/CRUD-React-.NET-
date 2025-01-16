using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
Env.Load();

// Build connection string from environment variables
var connectionString = $"Server={Environment.GetEnvironmentVariable("DATABASE_HOST")};" +
                      $"Port={Environment.GetEnvironmentVariable("DATABASE_PORT")};" +
                      $"Database={Environment.GetEnvironmentVariable("DATABASE_NAME")};" +
                      $"User={Environment.GetEnvironmentVariable("DATABASE_USERNAME")};" +
                      $"Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")};" +
                      "ConnectionTimeout=60;" +
                      "DefaultCommandTimeout=60;" +
                      "AllowUserVariables=True;" +
                      "SslMode=none;" +
                      "AllowPublicKeyRetrieval=True;" +
                      "Pooling=true;" +
                      "MinimumPoolSize=0;" +
                      "MaximumPoolSize=100;";

// Configure Kestrel to use specific ports
builder.WebHost.UseUrls("http://localhost:5000");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    
    options.UseMySql(connectionString, serverVersion, mySqlOptions =>
    {
        mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        );
        mySqlOptions.CommandTimeout(60);
        mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition");
    });
});

var app = builder.Build();

// Enable exception page in development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseRouting();

// Add CORS middleware - must be between UseRouting and UseEndpoints
app.UseCors("CorsPolicy");

// Add basic error handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        if (context.Response.HasStarted)
        {
            throw;
        }

        context.Response.Clear();
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            Message = "An error occurred",
            Error = ex.Message
        });

        Console.WriteLine($"Unhandled error: {ex}");
    }
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// Map controllers and add a test endpoint
app.MapControllers();
app.MapGet("/", () => "API is running!");
app.MapGet("/test", () => new { Message = "API is working", Time = DateTime.UtcNow });

app.Run();