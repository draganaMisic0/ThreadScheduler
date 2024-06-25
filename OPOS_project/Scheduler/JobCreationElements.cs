using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace OPOS_project.Scheduler
{

    public enum JobType
    {
        Blur,
        Sharpen,
        DetectEdges,
        Embossing,
        EqualizeHistogram
    }
    public class JobCreationElements
    {

        public string Name { get; private set; }
        public JobType? JobType { get; private set; }
        public DateTime? StartDateAndTime { get; private set; }

        public DateTime? Deadline { get; private set; }
        public int? TotalExecutionTime { get; private set; }
        public BitmapImage? Image {  get; private set; }
        
        public JobCreationElements(string Name, JobType? jobType, BitmapImage? image, 
            DateTime? startDateAndTime=null, DateTime? Deadline = null, int? TotalExecutionTime=null) 
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
