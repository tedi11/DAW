using System.ComponentModel.DataAnnotations;

namespace ProiectDAW.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string? ManagerId { get; set; }
        public string? ManagerEmail { get; set; }

        [Required(ErrorMessage = "Teapa, pune nume")]
        public string Name { get; set; }
        public virtual ICollection<Task>? Tasks { get; set; }
        public virtual ICollection<Member>? Members { get; set; } 
        
        //member e tabela asociativa intre proiect si user
        //ca sa vedem ce user e in ce proiect

    }
}
