using Application;
using Infrastructure.Adapters.Out.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddRazorPages()
    .WithRazorPagesRoot("/src/infrastructure/adapters/in/razor");

var app = builder.Build();

app.MapIdentityApi<IdentityUser>();   // expone /register, /login, /refresh, etc.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();