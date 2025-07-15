namespace ProjectPlaner.Models.Entity
{
    public class Client
    {
        public Guid clientId { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }

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
