using Application.Ports.In;
using Application.Ports.In.Iam;
using Application.UseCases;
using Application.UseCases.Iam;

namespace Application;

public static class ApplicationDepsContainer
{

  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<IHelloWorldPort, HelloWorldService>();
    services.AddScoped<IScopePort, ScopeService>();
    services.AddScoped<IRoleIamPort, RoleIamService>();
    services.AddScoped<IUserIamPort, UserIamService>();
    return services;
  }
  
}