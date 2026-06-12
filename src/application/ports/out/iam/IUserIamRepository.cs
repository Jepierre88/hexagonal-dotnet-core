using Domain.Iam;

namespace Application.Ports.Out.Iam;

public interface IUserIamRepository
{
    Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct = default);
    Task<AppUser?> GetByIdAsync(string userId, CancellationToken ct = default);

    /// <summary>Devuelve todos los scopes efectivos del usuario: propios + heredados de sus roles.</summary>
    Task<IReadOnlyList<Scope>> GetEffectiveScopesAsync(string userId, CancellationToken ct = default);

    Task AddUserScopeAsync(string userId, Guid scopeId, CancellationToken ct = default);
    Task RemoveUserScopeAsync(string userId, Guid scopeId, CancellationToken ct = default);
    Task<IReadOnlyList<Scope>> GetDirectScopesAsync(string userId, CancellationToken ct = default);
}
