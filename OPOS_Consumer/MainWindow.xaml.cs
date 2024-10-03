using OPOS_project.Scheduler;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace OPOS_Consumer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

       
        public static ManualResetEvent recievedScheduledJob = new ManualResetEvent(false);
        public static ManualResetEvent recievedUnscheduledJob = new ManualResetEvent(false);

        public static JobMessage unscheduledJob = null;
        public static JobMessage scheduledJob = null;

        public static Job currentJob = null;

        public static StackPanel windowStackPanel = null;
        public static ManualResetEvent jobCompletedEvent = new ManualResetEvent(false); // Initially not signaled
        public MainWindow()
        {
            InitializeComponent();
            MainWindow.windowStackPanel = this.myStackPanel;
            myStackPanel.Children.Clear();


            Consumer consumer = new Consumer();

     
            do
            {
                if(scheduledJob == null)
                {
                    consumer.ConsumeMessagesFromScheduled();
                }
                if(unscheduledJob == null)
                {
                    consumer.ConsumeMessagesFromUnscheduled();
                }

                if (scheduledJob!=null && unscheduledJob != null)
                {
                    DateTime currentDateTime = DateTime.Now;
                    TimeSpan difference = (TimeSpan)(currentDateTime - scheduledJob.StartDateAndTime);
                    if (Math.Abs(difference.TotalSeconds) < 3)
                    {
                        
                      
                     
                    }
                   
                }
             

            } while (scheduledJob != null || unscheduledJob != null);
            

            
        }

        public static void updateNewTaskPlayerControl(Job job)
        {
            windowStackPanel.Children.Clear();

            TaskPlayerControl newTpc = new TaskPlayerControl(Scheduler.getInstance().Schedule(currentJob.myJobElements));
            newTpc.Tag = currentJob.myJobElements;
            windowStackPanel.Children.Add(newTpc);
        }

        public static void setJobMessage(JobMessage message)
        {
             if(message == null)
            {
                return;
            }
            else if(message.isTimedJob) {
                if(MainWindow.scheduledJob == null)
                {
                    MainWindow.scheduledJob = message;
                    if (MainWindow.currentJob != null) {

                        jobCompletedEvent.WaitOne();
                    }
                    MainWindow.currentJob = JobFactory.createJob(message);
                    jobCompletedEvent.Reset();   //da se uradi wait 
                }
                else //this will trigger if the Job is still executing or not finished
                {
                    jobCompletedEvent.WaitOne(); //will wait if the job is not yet finished
                    MainWindow.scheduledJob = message;
                   
                    MainWindow.currentJob = JobFactory.createJob(message);
                    jobCompletedEvent.Reset();
                }
            }
            else
            {
                if(MainWindow.unscheduledJob == null)
                {
                    MainWindow.unscheduledJob = message;
                    if (MainWindow.currentJob != null)
                    {

                        jobCompletedEvent.WaitOne();
                    }
                    MainWindow.currentJob = JobFactory.createJob(message);
                    jobCompletedEvent.Reset();  // Reset the event to unsignaled state (for the next wait)
                }
                else //this will trigger if the unscheduledJob is still not finished
                {
                    jobCompletedEvent.WaitOne();
                    MainWindow.scheduledJob = message;
                   
                    MainWindow.currentJob = JobFactory.createJob(message);
                    jobCompletedEvent.Reset();
                }
            }
        }
    }
}