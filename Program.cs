var builder = WebApplication.CreateBuilder(args);

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