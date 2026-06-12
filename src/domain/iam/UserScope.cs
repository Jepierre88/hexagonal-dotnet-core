namespace Domain.Iam;

/// <summary>
/// Asignación directa de un Scope a un Usuario (además de los heredados por roles).
/// </summary>
public class UserScope
{
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;

    public Guid ScopeId { get; set; }
    public Scope Scope { get; set; } = null!;
}
