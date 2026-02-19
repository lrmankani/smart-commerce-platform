using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.MapGet("/" , () => "Product Service is running");

app.MapGet("/api/products", async (IConfiguration config) =>
{
    var cs = config.GetConnectionString("SqlDb");
    if(string.IsNullOrWhiteSpace(cs)   )
    {
        return Results.Problem("MIssing connectionstring");
    }
    try{
    using IDbConnection db = new SqlConnection(cs);
    var rows = await db.QueryAsync("select id , name , price, createdutc from dbo.products order by id desc");
    return Results.Ok(rows);
    }
    catch(Exception ex)
    {
    return Results.Problem(ex.Message);
}
    
});
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
