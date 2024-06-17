using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPOS_project.Specific_jobs;
namespace OPOS_project.Scheduler
{
    internal class JobFactory
    {

        Job createJob(JobCreationElements jobElements, int priority = 1) 
        { 
        
            switch(jobElements.JobType)
            {
                case JobType.Blur:
                    return new BlurImageJob(jobElements, priority); 

            }
        
        }
    }
}
