using OPOS_Consumer;

namespace OPOS_project.Scheduler
{
    public class Scheduler
    {
        private static int NUMBER_OF_THREADS = 100;

        private readonly PriorityQueue<Job, int> waitingJobs = new PriorityQueue<Job, int>();
        private readonly int maxNumOfRunningJobs;
        public  int currentNumOfRunningJobs = 0;
        //private int timedJobCount = 0;
        private readonly object _lock = new();
        public static Scheduler instance = null;

        public static Scheduler getInstance()
        {
            if (instance == null)
                instance = new Scheduler(NUMBER_OF_THREADS);
            return instance;
        }

        public Scheduler(int maxNumRunningJobs = 1)
        {
            this.maxNumOfRunningJobs = maxNumRunningJobs;
            startQueueChecker();
        }

        public Job Schedule(JobMessage jobElements)
        {
            Job newJob = JobFactory.createJob(jobElements);
            newJob.OnFinished = () => HandleFinishedJob(newJob);
          
            if (newJob.IsTimedJob)
            {
                //timedJobCount++;
                ScheduleTimedJob(newJob);
            }
            else
            {
                Schedule(newJob);
            }

            return newJob;
        }

        private void Schedule(Job newJob)
        {
            lock (_lock)
            {
                if (newJob.State == State.NotStarted)
                {
                    return;
                }
                else if (newJob.State == State.Finished)
                {
                    currentNumOfRunningJobs--;
                }
                else if (currentNumOfRunningJobs < maxNumOfRunningJobs)
                {
                    ResumeJob(newJob);
                }

            }
        }
        private Timer scheduledJobTimer = null;
        private void ScheduleTimedJob(Job newJob)
        {
            DateTime startTime = (DateTime)newJob.myJobElements.StartDateAndTime;
            TimeSpan ts = startTime - DateTime.Now;

            if (ts.TotalMilliseconds < 0 && (currentNumOfRunningJobs < maxNumOfRunningJobs))
            {
                StartJob(newJob);
            }
            else
            {
              
                scheduledJobTimer= new Timer(CheckBeginingTime, newJob, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            }
        }
        private void CheckBeginingTime(Object newJob) {

            Job job=(Job)newJob;    
            DateTime startTime = (DateTime)job.myJobElements.StartDateAndTime;
            TimeSpan ts = startTime - DateTime.Now;
            if (ts.TotalMilliseconds < 0 && (currentNumOfRunningJobs < maxNumOfRunningJobs))
            {
                StartJob(job);
            }
        }
       
        private Timer timer1 = null;
        

        private void startQueueChecker()
        {
            Task.Run(() =>
            {
                if (timer1 == null)
                {
                    timer1 = new Timer(CheckQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                    
                }
            });
        }
        private void endQueueChecker(Job job)
        {
            Task.Run(() =>
            {
                if (job.timer == null)
                {

                    job.timer = new Timer(IncrementExecutionTimer, job, TimeSpan.Zero, TimeSpan.FromSeconds(1));

                }
            });
        }
        private async void IncrementExecutionTimer(object job)
        {
            Job thisJob=(Job)job;
           
            lock (_lock)
            {
                
                thisJob.SecondsOfExecution++;
                if (State.Running.Equals(thisJob.State))
                {
                    if (thisJob.myJobElements.TotalExecutionTime != 0) 
                    {
                        if(thisJob.SecondsOfExecution > thisJob.myJobElements.TotalExecutionTime
                            || DateTime.Now > thisJob.myJobElements.Deadline)
                        {
                            this.StopJob(thisJob);
                        }
                    }

                }
                else
                {
                    ((Job)job).timer.Dispose();
                }
            }

        }
        private async void CheckQueue(object state)
        {

            lock (_lock)
            {
                if (waitingJobs.TryDequeue(out Job foundJob, out int priority)) //if there's job found in queue waiting
                {
                    if (foundJob != null) 
                    {
                        if (DateTime.Now >= foundJob.myJobElements.StartDateAndTime)
                        {
                            if (currentNumOfRunningJobs < maxNumOfRunningJobs)
                            {
                                StartJob(foundJob);
                            }
                            else
                            {
                                DateTime startTime = (DateTime)foundJob.myJobElements.StartDateAndTime;
                                TimeSpan ts = startTime - DateTime.Now;  
                                waitingJobs.Enqueue(foundJob, ts.Milliseconds);
                            }
                        }
                        else
                        {
                            DateTime startTime = (DateTime)foundJob.myJobElements.StartDateAndTime;
                            TimeSpan ts = startTime - DateTime.Now;

                            waitingJobs.Enqueue(foundJob, ts.Milliseconds);
                        }
                    }
                }
                else
                {
                    timer1.Dispose();   //timer stops when there are no more jobs in queue
                }
            }
        }

        public void StartJob(Job newJob)
        {
            lock (_lock)
            {
                if (currentNumOfRunningJobs < maxNumOfRunningJobs)
                {
                    if (newJob.State == State.NotStarted)
                    {
                        newJob.Start();
                        currentNumOfRunningJobs++;
                        if (newJob.IsTimedJob)
                        {
                            endQueueChecker(newJob);
                        }
                    }
                    else
                    {
                        
                        //Console.WriteLine("u nekom drugom je stanju a ne NOTStarted");
                    }
                }
                else
                {
                    //Console.WriteLine("nema slobodnih tredova za start");
                }
            }
        }

        public void ResumeJob(Job newJob)
        {
            lock (_lock)
            {
                if (currentNumOfRunningJobs < maxNumOfRunningJobs)
                {
                    
                    if (newJob.State == State.Paused)
                    {
                        newJob.Resume();

                        currentNumOfRunningJobs++;
                        if (newJob.IsTimedJob)
                        {
                            endQueueChecker(newJob);
                        }
                    }
                    else
                    {
                        //for all jobs
                        TimeSpan ts = (DateTime)(newJob.myJobElements.StartDateAndTime) - DateTime.Now;
                        //waitingJobs.Enqueue(newJob, ts.Milliseconds);

                    }
                }
                else
                {
                    
                    TimeSpan ts = DateTime.Now - (DateTime)(newJob.myJobElements.StartDateAndTime);
                    //waitingJobs.Enqueue(newJob, ts.Milliseconds);

                }
            }
        }

        public void checkPassedTime(Job job)
        {
            while (job.State == State.Running && job.IsTimedJob)
            {
                if (job.ExecutionTime > (int)job.myJobElements.TotalExecutionTime)
                {
                    StopJob(job);

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
                    if (newJob.IsTimedJob)
                    {
                        scheduledJobTimer.Dispose();
                    }
                }
                else if (newJob.State == State.NotStarted || newJob.State == State.Paused)
                {
                    newJob.Stop();
                    if (newJob.IsTimedJob)
                    {
                        scheduledJobTimer.Dispose();
                    }
                }
            }
        }


        private void signalCompletedJob(Job job)
        {
            if(job.IsTimedJob)
            {
                scheduledJobTimer.Dispose();
            }
            Task.Delay(Job.TIME_BETWEEN_NEW_JOB_DELAY).Wait();
            MainWindow.jobCompletedEvent.Set();
        }

        private void HandleFinishedJob(Job job)=> signalCompletedJob(job);
        private void HandleResumeRequest(Job job) => Schedule(job);
    }
}   