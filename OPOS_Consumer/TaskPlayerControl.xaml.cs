﻿using OPOS_project.Scheduler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace OPOS_Consumer
{
    public partial class TaskPlayerControl : UserControl
    {
        private Job myJob = null;
        private Scheduler scheduler = Scheduler.getInstance();

        public TaskPlayerControl()
        {
            InitializeComponent();
            this.progressBar.Value = 0;
            this.progressBar.Maximum = 100;
            this.jobName.Content = "[No job added yet]";
            myJob = null;
        }

        public TaskPlayerControl(Job job)
        {
            InitializeComponent();
            myJob = job;
            this.progressBar.Maximum = 100;
            this.progressBar.Value = 0;
            updateProgressBar();
            jobName.Content = job.myJobElements.Name;
            if (job.IsTimedJob) //Check to add the [Timed] tag to the name in the GUI
            {
                jobName.Content += "\n[Timed]";
            }
            else
            {
                jobName.Content += "\n[Manual]";
            }


            if (job.IsTimedJob && State.NotStarted.Equals(job.State))
            {
                this.playButton.IsEnabled = false;
            }

            messageLabel.Visibility = Visibility.Collapsed;
        }

        private async void updateProgressBar()
        {
            while (myJob.State != State.Finished)
            {
                if (myJob.State == State.Stopped)
                {
                    scheduler.StopJob(myJob);
                    //this.progressBar.Value = 0;
                    playButton.Visibility = Visibility.Hidden;
                    pauseButton.Visibility = Visibility.Hidden;
                    stopButton.Visibility = Visibility.Hidden;
                    messageLabel.Visibility = Visibility.Visible;
                    progressBar.Value = 0;
                    messageLabel.Content = "Job stoppped!";
                    return;
                }

                this.progressBar.Value = myJob.Progress;

                await Task.Delay(200);
            }
            if (myJob.State.Equals(State.Finished))
            {
                playButton.Visibility = Visibility.Hidden;
                pauseButton.Visibility = Visibility.Hidden;
                stopButton.Content = "Show Result";
                stopButton.Width = 93;
                messageLabel.Visibility = Visibility.Visible;
                messageLabel.Content = "Job finished!";
                stopButton.Margin = pauseButton.Margin;
                progressBar.Value = 100;
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && this.Tag is JobMessage myJobCreationElements)
            {
                if (myJob != null) //if myJob is already scheduled
                {
                    if (myJob.State == State.Paused)
                    {
                        scheduler.ResumeJob(myJob);
                    }
                    else
                    {
                        scheduler.StartJob(myJob);
                        playButton.Content = "Resume";
                    }
                }
                else   //if it was never scheduled
                {
                    myJob = scheduler.Schedule(myJobCreationElements);
                }
            }
        }

        private void pauseButtton_Click(object sender, object e)
        {
            if (sender is Button button && this.Tag is JobMessage)
            {
                scheduler.PauseJob(myJob);
                if (myJob.IsTimedJob && State.Paused.Equals(myJob.State))
                {
                    this.playButton.Content = "Resume";
                    this.playButton.IsEnabled = true;
                }
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && this.Tag is JobMessage myJobElements)
            {
                if (myJob.State.Equals(State.Finished))
                {
                    string relativePath = Job.RESULT_FILE_PATH + $"/{myJobElements.Name}.png";
                    string absolutePath = System.IO.Path.GetFullPath(relativePath);

                    ProcessStartInfo startInfo = new ProcessStartInfo(absolutePath)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);

                    return;
                }
                scheduler.StopJob(myJob);
            }
        }
    }
}