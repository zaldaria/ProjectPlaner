namespace ProjectPlaner.Models.Entity
{
    public class Project
    {
        public Guid projectId { get; set; }
        public string name { get; set; }
        public Client? client { get; set; }
        public Guid? clientId { get; set; }
        public string? comment { get; set; } // client comment
        public DateTime deadline { get; set; }
        public string description { get; set; }

        public List<Task> tasks { get; set; }

        public Project() { 
            name = string.Empty;
            //client = new Client();
            comment = string.Empty;
            deadline = new DateTime();
            description = string.Empty;
            tasks = new List<Task>();
        }
    }
}
