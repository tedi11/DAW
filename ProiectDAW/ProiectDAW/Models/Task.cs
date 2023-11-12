using System.ComponentModel.DataAnnotations;
using System;

namespace ProiectDAW.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title Required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description Required")]
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Deadline Required")]
        public DateTime Deadline { get; set; }
        [Required(ErrorMessage = "Content Required")]
        public string Content { get; set; }
        public string Status { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }  

        public string? UserId { get; set; } //punem string ca vine hash 
        public virtual AppUser? User { get; set; }


        public Task()
        {
            StartDate = DateTime.Now;
            Status = "Not Started";
            Content = "Placeholder"; //TODO
        }


    }
}
