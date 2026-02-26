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

var s3BaseUrl = "/static";
var pageTop = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Eduardo Osteicoechea</title>
    
    <link rel=""icon"" type=""image/x-icon"" href=""{s3BaseUrl}/media/favicon.ico"">
    
    <link rel=""stylesheet"" href=""{s3BaseUrl}/global.css"">
</head>
<body>";
var pageBottom = $@"
    <script src=""{s3BaseUrl}/global.js""></script>
</body>
</html>";

app.MapGet("/", () =>
{
    var html = $@"{pageTop}
    <h1>S3 Static Files Connected!</h1>
    <p>If the styling looks right and the console is clean, your pipeline is a success.</p>
    <h2>Testing nvim</p>
    {pageBottom}";

    return Results.Content(html, "text/html");
});

app.MapGet("/dbtest", async (IAmazonDynamoDB db) =>
{
    try
    {
        var response = await db.DescribeTableAsync("eduardoos-general-users");
        return Results.Ok(new
        {
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

app.MapGet("/federated_consequences", () =>
{
    var html = $@"{pageTop}
    
    <h1>Consecuencias Federadas</h1>
    <br>

    <h2>Santidad federada</h2>
    <p>""Porque el marido incrédulo es santificado en la mujer, y la mujer incrédula en el marido; pues de otra manera vuestros hijos serían inmundos, mientras que ahora son santos""</p>
    <p>1.Cor.7.14</p>
    <br>

    <h2>Juicio federado</h2>
    <p>""Y a sus hijos heriré de muerte, y todas las iglesias sabrán que yo soy el que escudriña la mente y el corazón; y os daré a cada uno según vuestras obras.""</p>
    <p>Ap.2.23</p>
    <br>

    <h2>Elección federada</h2>
    <p>""Y dijo Jeremías a la familia de los recabitas: Así ha dicho Jehová de los ejercitos, Dios de Israel: Por cuanto obedecísteis al mandamiento de Jonadab vuestro padre, y guardásteis todos sus mandamientos, e hicisteis conforme a todas las cosas que os mandó; por tanto, así ha dicho Jehová de los ejercitos, Dios de Israel: No faltará de Jonadab hijo de Recab un varon que esté en mi presencia todos los días""</p>
    <p>Jer.35.18-19</p>
    <br>

    <h2>Endurecimiento federado</h2>
    <p>""""</p>
    <p>Mat.13.10-17</p>
    <p>""""</p>
    <p>Rom.11.1-15</p>
    <p>""""</p>
    <p>Rom.1.18-32</p>
    <br>

    <h2>Corrupción federada</h2>
    <p>""""</p>
    <p>Rom.5.16-19</p>

    <h2>Justificación federada</h2>
    <p>""""</p>
    <p>Rom.5.16-19</p>

    <h2>Juicio federado</h2>
    <p>""""</p>
    <p>Gen.2</p>


    {pageBottom}";

    return Results.Content(html, "text/html");
});

app.Run();
