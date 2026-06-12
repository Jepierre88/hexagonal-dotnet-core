using System.Text;
using Application;
using Core.Middleware;
using Domain.Iam;
using Infrastructure.Adapters.Out.Persistence;
using Infrastructure.Adapters.Out.Persistence.Repositories.Iam;
using Infrastructure.Adapters.Out.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<BootstrapAdminOptions>(builder.Configuration.GetSection(BootstrapAdminOptions.SectionName));

builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services
    .AddIdentityCore<AppUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");

if (string.IsNullOrWhiteSpace(jwtOptions.Key))
{
    throw new InvalidOperationException("JWT signing key is missing.");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<Application.Ports.Out.Iam.IScopeRepository, ScopeRepository>();
builder.Services.AddScoped<Application.Ports.Out.Iam.IUserIamRepository, UserIamRepository>();
builder.Services.AddScoped<Application.Ports.Out.Iam.IRoleIamRepository, RoleIamRepository>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.RequireAdminRole, policy =>
        policy.RequireRole(AppRoles.Admin));
});

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddRazorPages()
    .WithRazorPagesRoot("/src/infrastructure/adapters/in/razor");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var bootstrapAdminOptions = scope.ServiceProvider.GetRequiredService<IOptions<BootstrapAdminOptions>>().Value;
    await IdentitySeedData.InitializeAsync(roleManager, userManager, bootstrapAdminOptions);
}

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();

app.MapGet("/", () => Results.Redirect("/home"));

app.MapControllers();
app.MapRazorPages();

app.Run();