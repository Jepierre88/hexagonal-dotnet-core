namespace Domain.Iam;

public class Scope
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<RoleScope> RoleScopes { get; set; } = [];
    public ICollection<UserScope> UserScopes { get; set; } = [];
}
