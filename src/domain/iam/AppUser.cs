using Microsoft.AspNetCore.Identity;

namespace Domain.Iam;

public class AppUser : IdentityUser
{
    public ICollection<UserScope> UserScopes { get; set; } = [];
}
