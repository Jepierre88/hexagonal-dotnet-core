using Application.Ports.Out.Iam;
using Domain.Iam;
using Infrastructure.Adapters.Out.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Out.Persistence.Repositories.Iam;

public class UserIamRepository(AppDbContext db) : IUserIamRepository
{
    private readonly AppDbContext _db = db;

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct = default)
        => await _db.Users.AsNoTracking().OrderBy(u => u.Email).ToListAsync(ct);

    public Task<AppUser?> GetByIdAsync(string userId, CancellationToken ct = default)
        => _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);

    public async Task<IReadOnlyList<Scope>> GetEffectiveScopesAsync(string userId, CancellationToken ct = default)
    {
        // Scopes directos del usuario
        var directScopeIds = await _db.UserScopes
            .Where(us => us.UserId == userId)
            .Select(us => us.ScopeId)
            .ToListAsync(ct);

        // Roles del usuario → scopes heredados
        var userRoleIds = await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);

        var roleScopeIds = await _db.RoleScopes
            .Where(rs => userRoleIds.Contains(rs.RoleId))
            .Select(rs => rs.ScopeId)
            .ToListAsync(ct);

        var allScopeIds = directScopeIds.Union(roleScopeIds).Distinct().ToList();

        return await _db.Scopes
            .AsNoTracking()
            .Where(s => allScopeIds.Contains(s.Id))
            .OrderBy(s => s.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Scope>> GetDirectScopesAsync(string userId, CancellationToken ct = default)
        => await _db.UserScopes
            .AsNoTracking()
            .Where(us => us.UserId == userId)
            .Select(us => us.Scope)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);

    public async Task AddUserScopeAsync(string userId, Guid scopeId, CancellationToken ct = default)
    {
        var alreadyExists = await _db.UserScopes
            .AnyAsync(us => us.UserId == userId && us.ScopeId == scopeId, ct);
        if (alreadyExists) return;

        _db.UserScopes.Add(new UserScope { UserId = userId, ScopeId = scopeId });
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveUserScopeAsync(string userId, Guid scopeId, CancellationToken ct = default)
    {
        var entry = await _db.UserScopes
            .FirstOrDefaultAsync(us => us.UserId == userId && us.ScopeId == scopeId, ct);
        if (entry is not null)
        {
            _db.UserScopes.Remove(entry);
            await _db.SaveChangesAsync(ct);
        }
    }
}
