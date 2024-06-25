using OPOS_project.Scheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.IO;

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

            messageLabel.Visibility = Visibility.Collapsed;
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

                    }
                    else
                    {
                        myJob.Start();
                        playButton.Content = "Resume";
                    }

                    while(myJob.State == State.Running)
                    { 
                        this.progressBar.Value=myJob.Progress;
                        
                        await Task.Delay(200);
                    }
                    if(myJob.State.Equals(State.Finished))
                    {
                        playButton.Visibility = Visibility.Hidden;
                        pauseButton.Visibility = Visibility.Hidden;
                        stopButton.Content = "Show Result";
                        stopButton.Width = 93; //Pause button width + gap + stop button width
                        messageLabel.Visibility = Visibility.Visible;
                        messageLabel.Content= "Job finished!";
                        stopButton.Margin = pauseButton.Margin;
                        progressBar.Value = 100;
                       
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
                if (myJob.State.Equals(State.Finished))
                {
                    string relativePath = Job.RESULT_FILE_PATH + $"/{myJob.myJobElements.Name}.png"; // Adjust the relative path as needed
                    string absolutePath = System.IO.Path.GetFullPath(relativePath);
                    
                    Console.WriteLine(absolutePath);
                    ProcessStartInfo startInfo = new ProcessStartInfo(absolutePath)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                   
                    return;
                }
                myJob.Stop();
                playButton.Visibility = Visibility.Hidden;
                pauseButton.Visibility = Visibility.Hidden;
                stopButton.Visibility = Visibility.Hidden;
                messageLabel.Visibility = Visibility.Visible;
                progressBar.Value = 0;
                messageLabel.Content = "Job stoppped!";

            }
            
        }
        
        private async void progressBar_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
                //progressBar.Value = myJobTag.Progress;
            

        }
    }
}
