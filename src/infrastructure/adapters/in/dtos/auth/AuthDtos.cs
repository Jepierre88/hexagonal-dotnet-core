using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Adapters.In.Dtos.Auth;

public sealed class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; init; } = string.Empty;
}

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

public sealed class AssignRoleRequest
{
    [Required]
    public string Role { get; init; } = string.Empty;
}

public sealed record AuthResponse(
    string AccessToken,
    string TokenType,
    string Email,
    DateTime ExpiresAtUtc,
    IEnumerable<string> Roles);

public sealed record AuthErrorResponse(string Message, IReadOnlyCollection<string> Errors);

public sealed record UserProfileResponse(string Id, string Email, IEnumerable<string> Roles, IEnumerable<string> Scopes);

public sealed record UserWithRolesResponse(string Id, string Email, IEnumerable<string> Roles);
