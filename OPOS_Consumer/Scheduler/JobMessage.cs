using Newtonsoft.Json;
using System.Drawing;
using System.IO;

namespace OPOS_project.Scheduler
{
    public enum JobType
    {
        Blur,
        Sharpen,
        DetectEdges,
        Embossing,
    }

    public class JobMessage
    {
        public string Name { get; private set; }
        public JobType? JobType { get; private set; }
        public DateTime? StartDateAndTime { get; private set; }

        public DateTime? Deadline { get; private set; }
        public int? TotalExecutionTime { get; private set; }
        public Bitmap Image { get; set; }
        public String BitmapPath { get; private set; }  
        public String json { get; set; }
        
        public JobMessage(string Name, JobType? jobType, String BitmapPath, 
            DateTime? startDateAndTime = null, DateTime? Deadline = null, int? TotalExecutionTime = null)
        {
            this.Name = Name;
            this.JobType = jobType;
            this.BitmapPath= BitmapPath;
            this.StartDateAndTime = startDateAndTime;
            this.Deadline = Deadline;
            this.TotalExecutionTime = TotalExecutionTime;
        }

        public Boolean isTimedJob { get
            {
                return this.StartDateAndTime != null || this.Deadline != null || this.TotalExecutionTime != null;
            } }

        

    }




}