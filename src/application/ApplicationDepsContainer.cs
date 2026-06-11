using Application.Ports.In;
using Application.UseCases;

namespace Application;

public static class ApplicationDepsContainer
{

  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<IHelloWorldPort, HelloWorldService>();
    return services;
  }
  
}