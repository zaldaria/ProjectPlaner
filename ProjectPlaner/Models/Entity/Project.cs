using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ProjectPlaner.Models.Entity
{
    public class Project
    {
        public Guid projectId { get; set; }

        [Required(ErrorMessage = "Project name is required.")]
        [StringLength(200, ErrorMessage = "Project name cannot exceed 200 characters.")]
        public string name { get; set; }
        public Client? client { get; set; }
        public Guid? clientId { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? comment { get; set; } // client comment

        [Required(ErrorMessage = "Time limit is required.")]
        [DataType(DataType.DateTime)]
        public DateTime deadline { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? description { get; set; }

        public List<Task> tasks { get; set; }
        public string? userId { get; set; }
        public IdentityUser? user { get; set; }

        public Project() { 
            name = string.Empty;            
            comment = string.Empty;
            deadline = DateTime.Now;
            description = string.Empty;
            tasks = new List<Task>();
        }
    }
}
