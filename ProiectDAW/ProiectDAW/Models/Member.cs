namespace ProiectDAW.Models
{
    public class Member
    {
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public string UserId { get; set; }
        public virtual AppUser User { get; set; }


    }
}
