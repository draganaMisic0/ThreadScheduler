using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project.Scheduler
{
    internal class Scheduler
    {
        private readonly PriorityQueue<Job, int> waitingJobs = new PriorityQueue<Job, int>();
        private readonly int maxNumOfRunningJobs;
        private  int currentNumOfRunningJobs = 0;
        private readonly object _lock = new();
        public Scheduler(int maxNumRunningJobs=2) 
        { 
            this.maxNumOfRunningJobs = maxNumRunningJobs;   
        }
        public Job Schedule(JobCreationElements jobElements)
        {
            Job newJob = JobFactory.createJob(jobElements);
            newJob.OnFinished = HandleFinishedJob;
            newJob.OnPaused = HandlePausedJob;
            newJob.OnStopped = HandleStoppedJob;
            newJob.OnResumeRequested = HandleResumeRequest;

            if(newJob.IsTimedJob) 
                ScheduleTimedJob(newJob);
            else
                Schedule(newJob);


            return newJob;

        }
        private void ScheduleTimedJob(Job newJob)
        {
            lock (_lock)
            {
                if (DateTime.Now.CompareTo((DateTime)newJob.myJobElements.StartDateAndTime) >= 0)
                {
                    Schedule(newJob);
                }
                else
                {
                    Task.Delay(DateTime.Now - (DateTime)newJob.myJobElements.StartDateAndTime).ContinueWith(_=> Schedule(newJob));
                }
            }
               
           
        }
        private void Schedule(Job newJob)
        {
            lock (_lock) 
            {
                if (currentNumOfRunningJobs < maxNumOfRunningJobs)
                {
                    currentNumOfRunningJobs++;
                    newJob.Start();
                }
                else 
                { 
                    waitingJobs.Enqueue(newJob, newJob.Priority);
                }
            }
        }
        private void ClearJobAndDequeue() 
        { 
            lock(_lock)
            {
                currentNumOfRunningJobs--;
                if(waitingJobs.TryDequeue(out Job? job, out int priority)) 
                { 
                    Schedule(job);
                }
            }
        }
        private void HandleFinishedJob() => ClearJobAndDequeue();
        private void HandleStoppedJob() => ClearJobAndDequeue();
        private void HandlePausedJob() => ClearJobAndDequeue();
        private void HandleResumeRequest(Job job) => Schedule(job);
     



    }
}
