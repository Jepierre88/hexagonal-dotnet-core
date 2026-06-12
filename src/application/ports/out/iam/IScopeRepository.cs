using Domain.Iam;

namespace Application.Ports.Out.Iam;

public interface IScopeRepository
{
    Task<IReadOnlyList<Scope>> GetAllAsync(CancellationToken ct = default);
    Task<Scope?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Scope?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<Scope> AddAsync(Scope scope, CancellationToken ct = default);
    Task<Scope> UpdateAsync(Scope scope, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
