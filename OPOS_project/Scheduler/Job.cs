﻿using System;
using System.CodeDom;
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
     abstract public class Job: IStatefulJob, IRunnableJob
    {
        public readonly static String RESULT_FILE_PATH = @"../../../Results";

        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(false);
        private State state = State.NotStarted;
        public int Progress { get; protected set; }
        //public string Name { get; set; } = "Job";

        private readonly object stateLock = new();
        public JobCreationElements myJobElements { get; private set; }
        public Boolean IsTimedJob { 
            get { return myJobElements.StartDateAndTime != null || myJobElements.Deadline != null 
                    || myJobElements.TotalExecutionTime!=null; }
            } 
        
        public int Priority { get; set; } = 1;
        public State State
        {
            get { return state; }
            set { state = value; }
        }
        internal Action OnPaused { get; set; } = () => { };
        internal Action OnStopped { get; set; } = () => { };
        internal Action OnFinished { get; set; } = () => { };
        internal Action<Job> OnResumeRequested { get; set; } = (Job job) => { };
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
                        OnResumeRequested(this);

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


            Action methodToRun = RunThisJob;
            Task.Run(methodToRun);
           

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
                        OnPaused();
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
                        OnResumeRequested(this);
                        pauseEvent.Set();
                       
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
                        OnStopped();
                        break;
                    case State.Running:
                        state = State.Stopped;
                        OnStopped();
                        break;
                 
                    case State.Paused:
                        state = State.Stopped;
                        OnStopped();
                        break;
                    
                    case State.Stopped:
                        throw new InvalidOperationException("Job cannot be stopped after being stopped.");
                    case State.Finished:
                        throw new InvalidOperationException("Job cannot be stopped after finishing.");
                }
            }
        }

        internal void Finish()
        {
            lock (stateLock)
            {
                switch (state)
                {
                    case State.NotStarted:
                        throw new InvalidOperationException("Job cannot finish before starting.");
                    case State.Running:
                        this.state = State.Finished;
                        OnFinished();
                        break;
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
                        break;
                    case State.Finished:
                        return;
                }
            }
        }
        public void checkState()
        {
            
                switch (state)
                {
                    case State.NotStarted:
                        throw new InvalidOperationException("Cannot check the state before starting the job");
                    case State.Running:
                        break;
                    case State.Paused:
                        OnPaused();
                        while(state == State.Paused)
                        {
                            pauseEvent.Wait();
                        }
                        break;
                    case State.Stopped:
                        state = State.Stopped;
                        OnStopped();
                        break;
                    case State.Finished:
                        throw new InvalidOperationException("Cannot check the state after finishing the job");
                }
            
        }

        abstract public void RunThisJob();
    }
}
