using System.Drawing;

namespace OPOS_project.Scheduler
{
    public enum JobType
    {
        Blur,
        Sharpen,
        DetectEdges,
        Embossing,
    }

    public class JobCreationElements
    {
        public string Name { get; private set; }
        public JobType? JobType { get; private set; }
        public DateTime? StartDateAndTime { get; private set; }

        public DateTime? Deadline { get; private set; }
        public int? TotalExecutionTime { get; private set; }
        public Bitmap? Image { get; private set; }

        public JobCreationElements(string Name, JobType? jobType, Bitmap? image,
            DateTime? startDateAndTime = null, DateTime? Deadline = null, int? TotalExecutionTime = null)
        {
            this.Name = Name;
            this.JobType = jobType;
            this.Image = image;
            this.StartDateAndTime = startDateAndTime;
            this.Deadline = Deadline;
            this.TotalExecutionTime = TotalExecutionTime;
        }
    }
}