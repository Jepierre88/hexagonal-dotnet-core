using Application.Ports.Out.Iam;
using Domain.Iam;
using Infrastructure.Adapters.Out.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Out.Persistence.Repositories.Iam;

public class RoleIamRepository(AppDbContext db) : IRoleIamRepository
{
    private readonly AppDbContext _db = db;

    public async Task<IReadOnlyList<IdentityRole>> GetAllAsync(CancellationToken ct = default)
        => await _db.Roles.AsNoTracking().OrderBy(r => r.Name).ToListAsync(ct);

    public Task<IdentityRole?> GetByIdAsync(string roleId, CancellationToken ct = default)
        => _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleId, ct);

    public async Task<IReadOnlyList<Scope>> GetScopesAsync(string roleId, CancellationToken ct = default)
        => await _db.RoleScopes
            .AsNoTracking()
            .Where(rs => rs.RoleId == roleId)
            .Select(rs => rs.Scope)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);

    public async Task AddRoleScopeAsync(string roleId, Guid scopeId, CancellationToken ct = default)
    {
        var alreadyExists = await _db.RoleScopes
            .AnyAsync(rs => rs.RoleId == roleId && rs.ScopeId == scopeId, ct);
        if (alreadyExists) return;

        _db.RoleScopes.Add(new RoleScope { RoleId = roleId, ScopeId = scopeId });
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveRoleScopeAsync(string roleId, Guid scopeId, CancellationToken ct = default)
    {
        var entry = await _db.RoleScopes
            .FirstOrDefaultAsync(rs => rs.RoleId == roleId && rs.ScopeId == scopeId, ct);
        if (entry is not null)
        {
            _db.RoleScopes.Remove(entry);
            await _db.SaveChangesAsync(ct);
        }
    }
}
