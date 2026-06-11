using Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddControllers();

builder.Services.AddRazorPages()
    .WithRazorPagesRoot("/src/infrastructure/adapters/in/razor");


var app = builder.Build();

app.MapControllers();
app.MapRazorPages();

app.Run();