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
        private Job myJob = null;
        public TaskPlayerControl()
        {
            InitializeComponent();
        }
    
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && this.Tag is JobCreationElements jobElements)
            {
                myJob = new Job(jobElements, 1);
                myJob.Start();
            }
        }

        private void pauseButtton_Click(object sender, object e)
        {
            if (sender is Button button && this.Tag is JobCreationElements jobElements)
            {
               
                myJob.Pause();
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && this.Tag is JobCreationElements jobElements)
            {
              
                myJob.Stop();
            }
        }
    }
}
