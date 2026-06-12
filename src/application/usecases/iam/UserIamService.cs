using Application.Ports.In.Iam;
using Application.Ports.Out.Iam;
using Domain.Iam;

namespace Application.UseCases.Iam;

public class UserIamService(IUserIamRepository userIamRepository, IScopeRepository scopeRepository) : IUserIamPort
{
    private readonly IUserIamRepository _userIamRepository = userIamRepository;
    private readonly IScopeRepository _scopeRepository = scopeRepository;

    public Task<IReadOnlyList<AppUser>> GetAllUsersAsync(CancellationToken ct = default)
        => _userIamRepository.GetAllAsync(ct);

    public async Task<IReadOnlyList<Scope>> GetUserEffectiveScopesAsync(string userId, CancellationToken ct = default)
    {
        await RequireUserExistsAsync(userId, ct);
        return await _userIamRepository.GetEffectiveScopesAsync(userId, ct);
    }

    public async Task<IReadOnlyList<Scope>> GetUserDirectScopesAsync(string userId, CancellationToken ct = default)
    {
        await RequireUserExistsAsync(userId, ct);
        return await _userIamRepository.GetDirectScopesAsync(userId, ct);
    }

    public async Task AssignScopeToUserAsync(string userId, Guid scopeId, CancellationToken ct = default)
    {
        await RequireUserExistsAsync(userId, ct);
        await RequireScopeExistsAsync(scopeId, ct);
        await _userIamRepository.AddUserScopeAsync(userId, scopeId, ct);
    }

    public async Task RemoveScopeFromUserAsync(string userId, Guid scopeId, CancellationToken ct = default)
    {
        await RequireUserExistsAsync(userId, ct);
        await _userIamRepository.RemoveUserScopeAsync(userId, scopeId, ct);
    }

    private async Task RequireUserExistsAsync(string userId, CancellationToken ct)
    {
        var user = await _userIamRepository.GetByIdAsync(userId, ct);
        if (user is null)
            throw new KeyNotFoundException($"Usuario '{userId}' no encontrado.");
    }

    private async Task RequireScopeExistsAsync(Guid scopeId, CancellationToken ct)
    {
        var scope = await _scopeRepository.GetByIdAsync(scopeId, ct);
        if (scope is null)
            throw new KeyNotFoundException($"Scope '{scopeId}' no encontrado.");
    }
}
