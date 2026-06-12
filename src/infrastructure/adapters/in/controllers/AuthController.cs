using System.Security.Claims;
using Application.Ports.Out.Iam;
using Domain.Iam;
using Infrastructure.Adapters.In.Dtos.Auth;
using Infrastructure.Adapters.Out.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Insfrastructure.Adapters.In;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IJwtTokenService jwtTokenService,
    IUserIamRepository userIamRepository) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IUserIamRepository _userIamRepository = userIamRepository;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var user = new AppUser { UserName = request.Email, Email = request.Email };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new AuthErrorResponse(
                "No se pudo registrar el usuario.",
                result.Errors.Select(e => e.Description).ToArray()));

        var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.User);
        if (!roleResult.Succeeded)
            return BadRequest(new AuthErrorResponse(
                "El usuario se creo pero no se pudo asignar el rol base.",
                roleResult.Errors.Select(e => e.Description).ToArray()));

        return Ok(await CreateAuthResponseAsync(user));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(new AuthErrorResponse("Credenciales invalidas.", []));

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(new AuthErrorResponse("Credenciales invalidas.", []));

        return Ok(await CreateAuthResponseAsync(user));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        var scopes = await _userIamRepository.GetEffectiveScopesAsync(user.Id);
        return Ok(new UserProfileResponse(user.Id, user.Email ?? string.Empty, roles, scopes.Select(s => s.Name)));
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
        var userClaims = await _userManager.GetClaimsAsync(user);
        var effectiveScopes = await _userIamRepository.GetEffectiveScopesAsync(user.Id);
        var scopeClaims = effectiveScopes.Select(s => new Claim("scope", s.Name));

        var token = _jwtTokenService.CreateToken(user, userClaims.Concat(roleClaims).Concat(scopeClaims));

        return new AuthResponse(token, "Bearer", user.Email ?? string.Empty, DateTime.UtcNow.AddHours(1), roles);
    }
}

[ApiController]
[Route("api/admin")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public class AdminController(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserWithRolesResponse>>> GetUsers()
    {
        var users = new List<UserWithRolesResponse>();
        foreach (var user in _userManager.Users.ToList())
        {
            var roles = await _userManager.GetRolesAsync(user);
            users.Add(new UserWithRolesResponse(user.Id, user.Email ?? string.Empty, roles));
        }
        return Ok(users);
    }

    [HttpPost("users/{userId}/roles")]
    public async Task<ActionResult<UserProfileResponse>> AssignRole(string userId, AssignRoleRequest request)
    {
        if (!AppRoles.All.Contains(request.Role))
            return BadRequest(new AuthErrorResponse("Rol no valido.", [$"Los roles permitidos son: {string.Join(", ", AppRoles.All)}"]));

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound(new AuthErrorResponse("Usuario no encontrado.", []));

        if (!await _roleManager.RoleExistsAsync(request.Role))
            return BadRequest(new AuthErrorResponse("El rol no existe en Identity.", []));

        if (await _userManager.IsInRoleAsync(user, request.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            return Ok(new UserProfileResponse(user.Id, user.Email ?? string.Empty, currentRoles, []));
        }

        var result = await _userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded)
            return BadRequest(new AuthErrorResponse(
                "No se pudo asignar el rol.",
                result.Errors.Select(e => e.Description).ToArray()));

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new UserProfileResponse(user.Id, user.Email ?? string.Empty, roles, []));
    }
}
