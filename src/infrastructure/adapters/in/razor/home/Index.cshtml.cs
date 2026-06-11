using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Insfrastructure.Adapters.In.Razor;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Nombre { get; set; } = "";

    public List<string> Saludos { get; set; } = new();

    public bool Enviado { get; private set; }

    public void OnGet()
    {
    }

    public async Task OnPostAsync()
    {
        await Task.Delay(1000); // Simula un proceso que toma tiempo
        Saludos = ListarSaludos(Nombre);
        Enviado = true;
        Console.WriteLine($"Hola, {Nombre}!");
    }

    private static List<string> ListarSaludos(string nombre) => new()
    {
        $"Hola, {nombre}!",
        $"¿Cómo estás, {nombre}?",
        $"¡Bienvenido, {nombre}!"
    };
}