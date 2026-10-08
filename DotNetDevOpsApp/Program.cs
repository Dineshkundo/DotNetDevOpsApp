var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        application = "DotNetDevOpsApp",
        timestamp = DateTime.UtcNow
    });
});

app.Run();
