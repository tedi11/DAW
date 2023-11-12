using Microsoft.AspNetCore.Identity;

namespace ProiectDAW.Models
{
    public class AppUser:IdentityUser
    {
       public virtual ICollection<Member> Members { get; set; }
    }
}
