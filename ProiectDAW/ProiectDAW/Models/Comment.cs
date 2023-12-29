using System.ComponentModel.DataAnnotations;
namespace ProiectDAW.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Message Required")]
        public string Message { get; set; }
        public int TaskId { get; set; }
        public virtual Task Task { get; set; }
        public string? UserId { get; set; }
        public virtual AppUser? Users { get; set; }


    }
}
