using OPOS_project.Specific_jobs;
using System.Drawing;
using System.IO;
namespace OPOS_project.Scheduler
{
    internal class JobFactory
    {
        public static Job createJob(JobMessage jobElements, int priority = 1)
        {

            if (File.Exists(jobElements.BitmapPath))
            {
                // Create a new Bitmap object from the file path.
                jobElements.Image = new Bitmap(jobElements.BitmapPath);

            }
            else
            {
                throw new FileNotFoundException("Image file not found", jobElements.BitmapPath);
            }

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