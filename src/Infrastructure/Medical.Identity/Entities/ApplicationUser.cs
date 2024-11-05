using Microsoft.AspNetCore.Identity;

namespace Medical.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
