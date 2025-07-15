using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectPlaner.Models.Entity
{
    public class Task
    {
        public Guid taskId { get; set; }
        public string? userId { get; set; }        
        public IdentityUser? user { get; set; }

        [Required(ErrorMessage = "Task name is required.")]
        [StringLength(100, ErrorMessage = "Task name cannot exceed 100 characters.")]
        public string name { get; set; }
        public Project? project {  get; set; } 
        public Guid? projectId { get; set; }        
        public TaskMarker marker { get; set; } 
        public TaskStatus status { get; set; }

        [Required(ErrorMessage = "Time limit is required.")]
        [DataType(DataType.DateTime)]
        public DateTime time_limit { get; set; }        
        public string? description { get; set; }

        public Task()
        {
            name = string.Empty;   
            marker = TaskMarker.NotSet;
            status = TaskStatus.NotSet;
            time_limit = DateTime.Now;            
        }

        public enum TaskMarker
        {
            [Display(Name = "Important and urgent")] ImportantUrgent, //0
            [Display(Name = "Important and not urgent")] ImportantNotUrgent, //1
            [Display(Name = "Not important and urgent")] NotImportantUrgent, //2
            [Display(Name = "Not important and not urgent")] NotImportantNotUrgent, //2
            [Display(Name = "Not Set")] NotSet, //4      
          
        }
        public string GetMarkerDisplayName()
        {
            var fieldInfo = marker.GetType().GetField(marker.ToString());
            var displayAttribute = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false)
                                .FirstOrDefault() as DisplayAttribute;

            return displayAttribute?.Name ?? marker.ToString() ?? "Not Set";
        }

        public enum TaskStatus
        {
            [Display(Name = "Not Set")] NotSet,
            [Display(Name = "Not Started")] NotStarted,
            [Display(Name = "In Progress")] InProgress,
            Stopped,
            Cancelled,
            Completed
        }
        
        public string GetStatusDisplayName()
        {
            var fieldInfo = status.GetType().GetField(status.ToString());
            var displayAttribute = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false)
                                .FirstOrDefault() as DisplayAttribute;

            return displayAttribute?.Name ?? status.ToString() ?? "Not Set";
        }

        public bool IsCompleted()
        {
            return status==TaskStatus.Completed;
        }
        public void SetCompleted()
        {
            status = TaskStatus.Completed;
        }

        public bool IsNotStarted()
        {
            return status == TaskStatus.NotStarted;
        }
        public void SetNotStarted()
        {
            status = TaskStatus.NotStarted;
        }

        public bool IsInProgress()
        {
            return status == TaskStatus.InProgress;
        }
        public void SetInProgress()
        {
            status = TaskStatus.InProgress;
        }

        public bool IsStopped()
        {
            return status == TaskStatus.Stopped;
        }
        public void SetStopped()
        {
            status = TaskStatus.Stopped;
        }

        public bool IsCancelled()
        {
            return status == TaskStatus.Cancelled;
        }
        public void SetCanceled()
        {
            status = TaskStatus.Cancelled;
        }

    }
}
