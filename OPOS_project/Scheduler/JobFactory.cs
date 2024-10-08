﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPOS_project.Specific_jobs;

namespace OPOS_project.Scheduler
{
    internal class JobFactory
    {

        public static Job createJob(JobCreationElements jobElements, int priority = 1) 
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
