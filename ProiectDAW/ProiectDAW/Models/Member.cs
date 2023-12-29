using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectDAW.Models
{
    public class Member
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? UserId { get; set; }
        public int? ProjectId { get; set; }

        public virtual AppUser? User { get; set; }
        public virtual Project? Project { get; set; }



    }
}
