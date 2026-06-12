using Microsoft.AspNetCore.Identity;

namespace Domain.Iam;

/// <summary>
/// Relación entre un Rol de Identity y un Scope.
/// Un rol hereda todos los scopes asignados.
/// </summary>
public class RoleScope
{
    public string RoleId { get; set; } = string.Empty;
    public IdentityRole Role { get; set; } = null!;

    public Guid ScopeId { get; set; }
    public Scope Scope { get; set; } = null!;
}
