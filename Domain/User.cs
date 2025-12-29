using Microsoft.AspNetCore.Identity;

namespace Domain;

public class User : IdentityUser
{
    public string? GoogleId { get; set; }
    public long? GithubId { get; set; }
}
