using OPOS_project.Specific_jobs;
using System.Drawing;
using System.Windows.Media.Imaging;
namespace OPOS_project.Scheduler
{
    internal class JobFactory
    {
        public static Job createJob(JobMessage jobElements, int priority = 1)
        {

            if (jobElements.JobType == JobType.Blur)
                return new BlurImageJob(jobElements, priority);
            else if (jobElements.JobType == JobType.DetectEdges)
                return new DetectEdgesJob(jobElements, priority);
            else if (jobElements.JobType == JobType.Embossing)
                return new EmbossingJob(jobElements, priority);
            else
                return new SharpenImageJob(jobElements, priority);
        }
    }
}