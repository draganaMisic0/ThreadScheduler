﻿using OPOS_project.Scheduler;
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

        Job myJob = null;
        Scheduler.Scheduler scheduler = OPOS_project.Scheduler.Scheduler.getInstance();
        
        public TaskPlayerControl(Job job)
        {
            InitializeComponent();
            myJob = job;
            this.progressBar.Maximum = 100;
            this.progressBar.Value = 0;
            updateProgressBar();

           

            messageLabel.Visibility = Visibility.Collapsed;
        }
    

        private async void updateProgressBar()
        {
            while (myJob.State != State.Finished)
            {

                this.progressBar.Value = myJob.Progress;

                
                await Task.Delay(200);
            }
            if (myJob.State.Equals(State.Finished))
            {
                playButton.Visibility = Visibility.Hidden;
                pauseButton.Visibility = Visibility.Hidden;
                stopButton.Content = "Show Result";
                stopButton.Width = 93; //Pause button width + gap + stop button width
                messageLabel.Visibility = Visibility.Visible;
                messageLabel.Content = "Job finished!";
                stopButton.Margin = pauseButton.Margin;
                progressBar.Value = 100;

            }
        }
    
        private async void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && this.Tag is JobCreationElements myJobCreationElements)
            {
                //ovdje tag treba da bude tipa Job
                Console.WriteLine("u play button");
                if (myJob != null)
                {
                    Console.WriteLine("ulazi u job nije null");

                    if (myJob.State == State.Paused) //Ovo je slucaj kada je Resume button
                    {
                        scheduler.ResumeJob(myJob);
                        

                    }
                    else
                    {
                        scheduler.StartJob(myJob);
                        playButton.Content = "Resume";
                    }

                    //updateProgressBar();
                    

                }

                else  //ako nije kreiran posao, kreira se i scheduluje
                {
                    Console.WriteLine("scheduluje job");
                    myJob = scheduler.Schedule(myJobCreationElements);
                }
            }
                
            }
        

        private void pauseButtton_Click(object sender, object e)
        {
            if (sender is Button button && this.Tag is JobCreationElements myJobElements)
            {
                scheduler.PauseJob(myJob);
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (sender is Button button && this.Tag is JobCreationElements myJobElements)
            {
                if (myJob.State.Equals(State.Finished))
                {
                    string relativePath = Job.RESULT_FILE_PATH + $"/{myJobElements.Name}.png"; // Adjust the relative path as needed
                    string absolutePath = System.IO.Path.GetFullPath(relativePath);
                    
                    Console.WriteLine(absolutePath);
                    ProcessStartInfo startInfo = new ProcessStartInfo(absolutePath)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                   
                    return;
                }
                scheduler.StopJob(myJob);
                playButton.Visibility = Visibility.Hidden;
                pauseButton.Visibility = Visibility.Hidden;
                stopButton.Visibility = Visibility.Hidden;
                messageLabel.Visibility = Visibility.Visible;
                progressBar.Value = 0;
                messageLabel.Content = "Job stoppped!";

            }
            
        }
        
       
    }
}
