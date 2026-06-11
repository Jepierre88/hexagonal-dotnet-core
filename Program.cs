using Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddControllers();



var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();