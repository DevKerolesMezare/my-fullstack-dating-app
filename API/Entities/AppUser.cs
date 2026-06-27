using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser  : IdentityUser
{
    public required string DisplayName{ set ;    get;}
    public string? ImageUrl {get;set;} 

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // Navigation property
    public Member Member { get; set; } = null!;
}
