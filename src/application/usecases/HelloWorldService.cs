using Application.Ports.In;

namespace Application.UseCases;

public class HelloWorldService : IHelloWorldPort
{
  public string SayHello(string name)
  {
    return $"Hello, {name}!";
  }
}