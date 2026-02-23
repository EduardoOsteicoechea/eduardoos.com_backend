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

app.MapGet("/", () => 
{
    // Make sure to replace YOUR_S3_BUCKET_URL with your actual public S3 bucket URL 
    // Example: https://eduardoos-static-files.s3.us-east-1.amazonaws.com
    var s3BaseUrl = "/static";

    var html = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Eduardo Osteicoechea</title>
    
    <link rel=""icon"" type=""image/x-icon"" href=""{s3BaseUrl}/media/favicon.ico"">
    
    <link rel=""stylesheet"" href=""{s3BaseUrl}/global.css"">
</head>
<body>
    <h1>S3 Static Files Connected!</h1>
    <p>If the styling looks right and the console is clean, your pipeline is a success.</p>
    <h2>Testing nvim</p>
    <script src=""{s3BaseUrl}/global.js""></script>
</body>
</html>";

    return Results.Content(html, "text/html");
});

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
