using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OPOS_project.Scheduler
{

    public enum JobType
    {
        Blur,
        Sharpen,
        DetectEdges,
        EqualizeHistogram
    }
    public class JobCreationElements
    {

        public string Name { get; private set; }
        public JobType? JobType { get; private set; }
        public DateTime? StartDateAndTime { get; private set; }
        public Image? Image {  get; private set; }
        

        public JobCreationElements(string Name, JobType? jobType, DateTime? startDateAndTime, Image? image)
        {
            this.Name = Name;
            this.JobType = jobType;
            this.StartDateAndTime = startDateAndTime;
            this.Image = image;
        }
        
    }
}
