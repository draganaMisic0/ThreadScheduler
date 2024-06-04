using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project.Scheduler
{
    public enum State
    {
        NotStarted,
        Running,
        Paused,
        Stopped,
        Finished
    }
     public class Job
    {
        private State state = State.NotStarted;
        public string Name { get; set; } = "Job";

        private readonly object stateLock = new();
        private readonly JobCreationElements myJobElements;
        public int Priority { get; set; } = 1;
        public State State
        {
            get { return state; }
            set { state = value; }
        }
        public Job(JobCreationElements myJobElements, int priority)
        {
            this.myJobElements = myJobElements;
            this.Priority = priority;
        }
        public void Start() {

            lock (stateLock)
            {
           
                switch (state)
                {
                    case State.NotStarted:
                        state = State.Running;
                        break;
                    case State.Running:
                        throw new InvalidOperationException("Job cannot be started in the running state.");
                    /*case State.RunningWithPauseRequest:
                        throw new InvalidOperationException("Job cannot be started in the running with pause request state.");*/
                    case State.Paused:
                        throw new InvalidOperationException("Job cannot be started in the paused state.");
                    /*case JobState.WaitingToResume:
                        state = JobState.Running;
                        Monitor.Pulse(_lock);
                        break;*/
                    case State.Stopped:
                        throw new InvalidOperationException("Job cannot be started in the stopped state.");
                    case State.Finished:
                        throw new InvalidOperationException("Job cannot be started in the finished state.");
                }
            }
            Task.Run(async () =>
            {
                Console.WriteLine("neki posao se izvrsava");

                lock (stateLock)
                {
                    State = State.Finished;
                }
            });
            ss
        }
        internal void Pause()
        {
            lock (stateLock)
            {
                switch (state)
                {
                    case State.NotStarted:
                        // TODO Handle
                        break;
                    /*case State.Running:
                        state = State.RunningWithPauseRequest;
                        break;
                    case State.RunningWithPauseRequest:
                        break;
                    */
                    case State.Paused:
                        break;
                    /*case State.WaitingToResume:
                        // TODO Handle
                        break;
                    */
                    case State.Stopped:
                        throw new InvalidOperationException("Job cannot be paused after being stopped.");
                    case State.Finished:
                        throw new InvalidOperationException("Job cannot be paused after finishing.");
                }
            }
        }

        internal void RequestResume()
        {
            lock (stateLock)
            {
                switch (state)
                {
                    case State.NotStarted:
                        throw new InvalidOperationException("Job cannot be resumed before starting.");
                    case State.Running:
                        break;
                    /*case State.RunningWithPauseRequest:
                        state = State.Running;
                        break;
                    */
                    /*case State.Paused:
                        state = State.WaitingToResume;
                        OnResumeRequested(this);
                        break;
                    */
                    /*case State.WaitingToResume:
                        break;
                    */
                    case State.Stopped:
                        throw new InvalidOperationException("Job cannot be resumed after being stopped.");
                    case State.Finished:
                        throw new InvalidOperationException("Job cannot be resumed after finishing.");
                }
            }
        }

        internal void Stop()
        {
            lock (stateLock)
            {
                switch (state)
                {
                    case State.NotStarted:
                        state = State.Stopped;
                        break;
                    case State.Running:
                        state = State.Stopped;
                        break;
                   
                   /* case JobState.RunningWithPauseRequest:
                        state = JobState.RunningWithStopRequest;
                        break;
                    case JobState.RunningWithStopRequest:
                        break;
                   */
                    case State.Paused:
                        state = State.Stopped;
                        break;
                    
                    case State.Stopped:
                        throw new InvalidOperationException("Job cannot be stopped after being stopped.");
                    case State.Finished:
                        throw new InvalidOperationException("Job cannot be stopped after finishing.");
                }
            }
        }

        private void Finish()
        {
            lock (stateLock)
            {
                switch (state)
                {
                    case State.NotStarted:
                        throw new InvalidOperationException("Job cannot finish before starting.");
                    case State.Running:
                    //case JobState.RunningWithPauseRequest:
                   /* case JobState.RunningWithStopRequest:
                        state = JobState.Finished;
                        OnFinished();
                        break;
                   */
                    case State.Paused:
                        throw new InvalidOperationException("Job cannot finish while paused.");
                    /*case JobState.WaitingToResume:
                        throw new InvalidOperationException("Job cannot finish while in waiting state.");
                    */
                    case State.Stopped:
                        throw new InvalidOperationException("Job cannot finish after being stopped.");
                    case State.Finished:
                        return;
                }
            }
        }





    }
}
