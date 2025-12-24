using Microsoft.AspNetCore.Identity;

namespace Domain;

public class User : IdentityUser
{
    public string GoogleId { get; set; } = null!;
    public AuthProvider? Provider { get; set; }
}
