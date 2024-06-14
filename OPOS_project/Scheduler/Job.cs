using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(false);
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
                    
                    case State.Paused:
                        throw new InvalidOperationException("Job cannot be started in the paused state.");
                   
                    case State.Stopped:
                        throw new InvalidOperationException("Job cannot be started in the stopped state.");
                    case State.Finished:
                        throw new InvalidOperationException("Job cannot be started in the finished state.");
                }
            }
            Task.Run(async () =>
            {


                Console.WriteLine("ispisivanje stanja unutar taska " + state.ToString());
                while (state == State.Paused)
                {
                    pauseEvent.Wait();

                }

                Console.WriteLine("izlazi iz if");


                if (state == State.Stopped)
                {
                    Console.WriteLine("stopped ");
                    return;
                }
                Console.WriteLine("Iteracija: " + i);
                Thread.Sleep(1000);


            
                lock (stateLock)
                {
                    State = State.Finished;
                }
            });
           
        }
        internal void Pause(){


            Console.WriteLine(State.ToString());
            lock (stateLock)
            {
                switch (state)
                {
                    case State.NotStarted:
                        throw new InvalidOperationException("Job cannot be paused before it started");
                     
                    case State.Running:
                        state = State.Paused;
                        pauseEvent.Set();
                        break;
                   
                    case State.Paused:
                        break;
                  
                    case State.Stopped:
                        throw new InvalidOperationException("Job cannot be paused after being stopped.");
                    case State.Finished:
                        throw new InvalidOperationException("Job cannot be paused after finishing.");
                }
            }
            Console.WriteLine("Posao je pauziran");
        }

        internal void Resume()
        {
            lock (stateLock)
            {
                switch (state)
                {
                    case State.NotStarted:
                        throw new InvalidOperationException("Job cannot be resumed before starting.");
                    case State.Running:
                        break;
                   
                    case State.Paused:

                        state = State.Running;
                       

                        pauseEvent.Reset();
                        Console.WriteLine(this.state);
                        Console.WriteLine("pulsirano");
                        break;
                       
                      
                  
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
