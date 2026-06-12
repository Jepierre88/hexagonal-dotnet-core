using Application.Ports.Out.Iam;
using Domain.Iam;
using Infrastructure.Adapters.Out.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Out.Persistence.Repositories.Iam;

public class ScopeRepository(AppDbContext db) : IScopeRepository
{
    private readonly AppDbContext _db = db;

    public async Task<IReadOnlyList<Scope>> GetAllAsync(CancellationToken ct = default)
        => await _db.Scopes.AsNoTracking().OrderBy(s => s.Name).ToListAsync(ct);

    public Task<Scope?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Scopes.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<Scope?> GetByNameAsync(string name, CancellationToken ct = default)
        => _db.Scopes.AsNoTracking().FirstOrDefaultAsync(s => s.Name == name, ct);

    public async Task<Scope> AddAsync(Scope scope, CancellationToken ct = default)
    {
        _db.Scopes.Add(scope);
        await _db.SaveChangesAsync(ct);
        return scope;
    }

    public async Task<Scope> UpdateAsync(Scope scope, CancellationToken ct = default)
    {
        _db.Scopes.Update(scope);
        await _db.SaveChangesAsync(ct);
        return scope;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var scope = await _db.Scopes.FindAsync([id], ct);
        if (scope is not null)
        {
            _db.Scopes.Remove(scope);
            await _db.SaveChangesAsync(ct);
        }
    }
}
