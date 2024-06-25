using OPOS_project.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OPOS_project
{
    /// <summary>
    /// Interaction logic for TaskPlayerControl.xaml
    /// </summary>
    public partial class TaskPlayerControl : UserControl
    {

        Job myJobTag = null;
        
        public TaskPlayerControl()
        {
            InitializeComponent();
            this.progressBar.Maximum = 100;
            this.progressBar.Value = 0;
        }
    
        private async void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && this.Tag is Job myJob)
            {                                       //ovdje tag treba da bude tipa Job
                
                if (myJob != null)
                {   //ovdje ne kreiram job jer je kreiran u new window konstruktoru 
                    //myJob = new Job(jobElements, 1);
                    myJobTag = myJob;

                    
                    //this.DataContext = myJobTag.Progress;


                    if (myJob.State == State.Paused) //Ovo je slucaj kada je Resume button
                    {
                        myJob.Resume();
                        while (myJob.State == State.Running)
                        {
                            this.progressBar.Value = myJob.Progress;

                            await Task.Delay(200);
                        }
                        return;

                    }
                    // myJob.Start(myJob.RunThisJob);
                    playButton.Content = "Resume";
                    myJob.Start();

                    while(myJob.State == State.Running)
                    { 
                        this.progressBar.Value=myJob.Progress;
                        
                        await Task.Delay(200);
                    } 
                }

                
            }
        }

        private void pauseButtton_Click(object sender, object e)
        {
            if (sender is Button button && this.Tag is Job myJob)
            {
               
                myJob.Pause();
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && this.Tag is Job myJob)
            {
              
                myJob.Stop();
            }
        }

        private async void progressBar_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
                //progressBar.Value = myJobTag.Progress;
            

        }
    }
}
