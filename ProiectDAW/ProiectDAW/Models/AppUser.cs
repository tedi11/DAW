using Microsoft.AspNetCore.Identity;

namespace ProiectDAW.Models
{
    public class AppUser:IdentityUser
    {
        public virtual ICollection<Member>? Members { get; set; }
        public virtual ICollection<Project>? Projects { get; set; }
        public virtual ICollection<Comment>? Coments { get; set; }
        public virtual ICollection<Task>? Tasks { get; set; }
    }
}
