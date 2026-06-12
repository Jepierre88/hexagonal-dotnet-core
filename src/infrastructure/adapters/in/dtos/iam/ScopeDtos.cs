using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Adapters.In.Dtos.Iam;

public sealed record ScopeResponse(Guid Id, string Name, string? Description);

public sealed class CreateScopeRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public sealed class UpdateScopeRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}
