using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string? Status { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }

        public string? UserId { get; set; } //punem string ca vine hash 
        public virtual AppUser? User { get; set; }

        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Statuses { get; set; }




    }
}
