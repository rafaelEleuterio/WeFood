using Microsoft.AspNetCore.Identity;
using WeFood.Domain.Enums;

namespace Infraestructure.Identity;
public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; }
    public UserType UserType { get; set; }
}
