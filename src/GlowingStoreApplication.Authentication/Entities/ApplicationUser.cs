using Microsoft.AspNetCore.Identity;

namespace GlowingStoreApplication.Authentication.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
}