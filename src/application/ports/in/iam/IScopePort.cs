using Domain.Iam;

namespace Application.Ports.In.Iam;

public interface IScopePort
{
    Task<IReadOnlyList<Scope>> GetAllScopesAsync(CancellationToken ct = default);
    Task<Scope> GetScopeAsync(Guid id, CancellationToken ct = default);
    Task<Scope> CreateScopeAsync(string name, string? description, CancellationToken ct = default);
    Task<Scope> UpdateScopeAsync(Guid id, string name, string? description, CancellationToken ct = default);
    Task DeleteScopeAsync(Guid id, CancellationToken ct = default);
}
