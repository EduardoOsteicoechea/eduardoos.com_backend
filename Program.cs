using Amazon.DynamoDBv2;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemsManager("/eduardoos/backend/");

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();

var jwtSecret = builder.Configuration["JWT_SECRET"];

if (string.IsNullOrEmpty(jwtSecret))
{
    throw new Exception("CRITICAL: JWT_SECRET not found in AWS Parameter Store!");
}
else
{
    Console.WriteLine("Found JWT!");
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

app.MapGet("/", () => new { Message = "Working" });

app.MapGet("/dbtest", async (IAmazonDynamoDB db) =>
{
    try 
    {
        var response = await db.DescribeTableAsync("eduardoos-general-users");
        return Results.Ok(new { 
            Status = "Connected", 
            TableName = response.Table.TableName,
            ItemCount = response.Table.ItemCount 
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"DynamoDB Error: {ex.Message}");
    }
});

app.Run();