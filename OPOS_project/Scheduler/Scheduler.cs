using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project.Scheduler
{
    internal class Scheduler
    {
        private static int NUMBER_OF_THREADS = 3;

        private readonly PriorityQueue<Job, int> waitingJobs = new PriorityQueue<Job, int>();
        private readonly int maxNumOfRunningJobs;
        private int currentNumOfRunningJobs = 0;
        private readonly object _lock = new();
        static private Scheduler instance = null;
        static public Scheduler getInstance()
        {
            if (instance == null)
                instance = new Scheduler(NUMBER_OF_THREADS);
            return instance;
        }
        public Scheduler(int maxNumRunningJobs = 2)
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

            if (newJob.IsTimedJob)
            {
                ScheduleTimedJob(newJob);
            }
            else
            {
                Schedule(newJob);
            }


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
                    Task.Delay(DateTime.Now - (DateTime)newJob.myJobElements.StartDateAndTime).ContinueWith(_ => Schedule(newJob));
                }
            }


        }
        public void StartJob(Job newJob)
        {
            lock (_lock)
            {
                if (currentNumOfRunningJobs < maxNumOfRunningJobs)
                {
                    Console.WriteLine("broj tredova dobar");
                    if (newJob.State == State.NotStarted)
                    {
                        Console.WriteLine("hoce da ga startuje iz schedulera");
                        newJob.Start();
                        currentNumOfRunningJobs++;

                    }
                    else
                    {

                        Console.WriteLine("u nekom drugom je stanju a ne NOTStarted");
                    }
                }
                else
                {

                    Console.WriteLine("nema slobodnih tredova za start");
                }
            }
        }

        public void ResumeJob(Job newJob)
        {
            lock (_lock)
            {
                Console.WriteLine("broj tredova "+currentNumOfRunningJobs);
                if (currentNumOfRunningJobs < maxNumOfRunningJobs)
                {
                    
                    Console.WriteLine("broj tredova dobar");
                    if (newJob.State == State.Paused)
                    {
                        Console.WriteLine("hoce da ga resumuje iz schedulera");
                        newJob.Resume();
                        currentNumOfRunningJobs++;
                    }
                    else
                    {
                        waitingJobs.Enqueue(newJob, newJob.Priority);
                        Console.WriteLine("u nekom drugom je stanju a nePaused ");
                    }
                }
                else
                {
                    waitingJobs.Enqueue(newJob, newJob.Priority);

                    Console.WriteLine("nema slobodnih tredova za start");
                }
            }
        }

        public void PauseJob(Job newJob)
        {
            lock (_lock)
            {
                if (newJob.State == State.Running)
                {
                    newJob.Pause();
                    currentNumOfRunningJobs--;
                }
                else if (newJob.State == State.NotStarted)
                {
                    return;
                }
            }
        }
        public void StopJob(Job newJob)
        {
            lock (_lock)
            {
                if (newJob.State == State.Running)
                {
                    newJob.Stop();
                    currentNumOfRunningJobs--;
                }
                else if (newJob.State == State.NotStarted || newJob.State == State.Paused)
                {
                    newJob.Stop();
                }
            }
        }
        private void Schedule(Job newJob)
        {
            lock (_lock)
            {
                if (newJob.State == State.NotStarted)
                {
                    //waitingJobs.Enqueue(newJob, newJob.Priority);
                    return;
                }
                if (newJob.State == State.Finished)
                {
                    currentNumOfRunningJobs--;
                }
                else if (currentNumOfRunningJobs < maxNumOfRunningJobs)
                {
                    //currentNumOfRunningJobs++;
                    newJob.Resume();
                }
                else
                {
                    // waitingJobs.Enqueue(newJob, newJob.Priority);
                }
            }
        }
        private void ClearJobAndDequeue()
        {
            lock (_lock)
            {

                //currentNumOfRunningJobs--;
                //if(waitingJobs.TryDequeue(out Job? job, out int priority)) 
                //{
                //    if (State.Finished.Equals(job.State) )
                //    {
                //        return;
                //    }
                //    Schedule(job);
            }
        }

        private void UpdateNumOfCurrentRunningJobs()
        {
            lock (_lock)
            {
                currentNumOfRunningJobs--;
                Console.WriteLine("job finished " + currentNumOfRunningJobs);
            }
        }

        private void HandleFinishedJob() => UpdateNumOfCurrentRunningJobs();
        private void HandleStoppedJob() => ClearJobAndDequeue();
        private void HandlePausedJob() => ClearJobAndDequeue();
        private void HandleResumeRequest(Job job) => Schedule(job);
     



    }
}
