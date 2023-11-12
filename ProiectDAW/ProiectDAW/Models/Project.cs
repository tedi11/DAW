using System.ComponentModel.DataAnnotations;

namespace ProiectDAW.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Manager Id Required")]
        public int ManagerId { get; set; }
        [Required(ErrorMessage = "Project Name Required")]
        public string Name { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Member> Members { get; set; } 
        
        //member e tabela asociativa intre proiect si user
        //ca sa vedem ce user e in ce proiect

    }
}
