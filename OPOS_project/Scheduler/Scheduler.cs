namespace OPOS_project.Scheduler
{
    internal class Scheduler
    {
        private static int NUMBER_OF_THREADS = 3;

        private readonly PriorityQueue<Job, int> waitingJobs = new PriorityQueue<Job, int>();
        private readonly int maxNumOfRunningJobs;
        private int currentNumOfRunningJobs = 0;
        //private int timedJobCount = 0;
        private readonly object _lock = new();
        private static Scheduler instance = null;

        public static Scheduler getInstance()
        {
            if (instance == null)
                instance = new Scheduler(NUMBER_OF_THREADS);
            return instance;
        }

        private Scheduler(int maxNumRunningJobs = 2)
        {
            this.maxNumOfRunningJobs = maxNumRunningJobs;
            startQueueChecker();
        }

        public Job Schedule(JobCreationElements jobElements)
        {
            Job newJob = JobFactory.createJob(jobElements);
            newJob.OnFinished = HandleFinishedJob;
          
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
                lock (_lock)
                {
                    waitingJobs.Enqueue(newJob, ts.Milliseconds);
                }
            }
        }

        private Timer timer = null;

        private void startQueueChecker()
        {
            Task.Run(() =>
            {
                if (timer == null)
                {
                    timer = new Timer(CheckQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                }
            });
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
                    timer.Dispose();   //timer stops when there are no more jobs in queue
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
                        //timedJobCount--;
                    }
                }
                else if (newJob.State == State.NotStarted || newJob.State == State.Paused)
                {
                    newJob.Stop();
                }
            }
        }


        private void UpdateNumOfCurrentRunningJobs()
        {
            lock (_lock)
            {
                currentNumOfRunningJobs--;
            }
        }

        private void HandleFinishedJob() => UpdateNumOfCurrentRunningJobs();
        private void HandleResumeRequest(Job job) => Schedule(job);
    }
}