namespace ProjectPlaner.Models.Entity
{
    public class Mark
    {
        public Guid markId { get; set; }
        public string name { get; set; }
        public string color { get; set; }

        public List<Task> tasks { get; set; }

        public Mark()
        {
            name = string.Empty;
            color = "#999999";
            tasks = new List<Task>();
        }
    }
}
