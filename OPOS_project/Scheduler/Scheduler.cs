using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project.Scheduler
{
    internal class Scheduler
    {
        Queue<Job> jobs = new Queue<Job>();
        public int MaxNumOfRunningJobs { get; set; }
        private int CurrentNumOfRunningJobs { get; set; }
        public Scheduler() { }
        public void AddJob(Job job)  {   jobs.Enqueue(job);  }
        public void RemoveJob(Job job) {  }
        public void LaunchNextJob() {
            if (jobs.TryDequeue(out Job result)) { 
                CurrentNumOfRunningJobs++;
                result.Start();
            } }
        public void Clear() { jobs.Clear(); }
        public void Run() { }
        public void Stop() { }
       



    }
}
