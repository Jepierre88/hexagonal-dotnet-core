using Application.Ports.In.Iam;
using Application.Ports.Out.Iam;
using Domain.Iam;

namespace Application.UseCases.Iam;

public class ScopeService(IScopeRepository scopeRepository) : IScopePort
{
    private readonly IScopeRepository _scopeRepository = scopeRepository;

    public Task<IReadOnlyList<Scope>> GetAllScopesAsync(CancellationToken ct = default)
        => _scopeRepository.GetAllAsync(ct);

    public async Task<Scope> GetScopeAsync(Guid id, CancellationToken ct = default)
    {
        var scope = await _scopeRepository.GetByIdAsync(id, ct);
        if (scope is null)
            throw new KeyNotFoundException($"Scope '{id}' no encontrado.");
        return scope;
    }

    public async Task<Scope> CreateScopeAsync(string name, string? description, CancellationToken ct = default)
    {
        var existing = await _scopeRepository.GetByNameAsync(name, ct);
        if (existing is not null)
            throw new InvalidOperationException($"Ya existe un scope con el nombre '{name}'.");

        var scope = new Scope
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };
        return await _scopeRepository.AddAsync(scope, ct);
    }

    public async Task<Scope> UpdateScopeAsync(Guid id, string name, string? description, CancellationToken ct = default)
    {
        var scope = await _scopeRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Scope '{id}' no encontrado.");

        var duplicate = await _scopeRepository.GetByNameAsync(name, ct);
        if (duplicate is not null && duplicate.Id != id)
            throw new InvalidOperationException($"Ya existe un scope con el nombre '{name}'.");

        scope.Name = name;
        scope.Description = description;
        return await _scopeRepository.UpdateAsync(scope, ct);
    }

    public async Task DeleteScopeAsync(Guid id, CancellationToken ct = default)
    {
        var scope = await _scopeRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Scope '{id}' no encontrado.");
        await _scopeRepository.DeleteAsync(scope.Id, ct);
    }
}
