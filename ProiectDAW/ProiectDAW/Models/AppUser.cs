using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectDAW.Models
{
    public class AppUser:IdentityUser
    {

        public virtual ICollection<Member>? Members { get; set; }
        public virtual ICollection<Project>? Projects { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
        public virtual ICollection<Task>? Tasks { get; set; }
        public int? Test {  get; set; }
    }
}
