using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectPlaner.Models.Entity
{
    public class Client
    {
        public Guid clientId { get; set; }

        [Required(ErrorMessage = "Client name is required.")]
        [StringLength(100, ErrorMessage = "Client name cannot exceed 100 characters.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        //[Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string phone { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        //[EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(150, ErrorMessage = "Email address cannot exceed 150 characters.")]
        public string email { get; set; }
        public string? userId { get; set; }
        public IdentityUser? user { get; set; }

        public List<Project> projects { get; set; }

        public Client()
        {
            name = string.Empty;
            phone = string.Empty;
            email = string.Empty;
            projects = new List<Project>();
        }
    }
}
