var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemsManager("/eduardoos/backend/");

var jwtSecret = builder.Configuration["JWT_SECRET"];

if (string.IsNullOrEmpty(jwtSecret))
{
    throw new Exception("CRITICAL: JWT_SECRET not found in AWS Parameter Store!");
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://eduardoos.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.MapGet("/api/hello", () => new { Message = "Hello from .NET 9 JIT!" });

app.Run();