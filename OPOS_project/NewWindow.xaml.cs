using OPOS_project.Scheduler;
using System.Windows;

namespace OPOS_project
{
    public partial class NewWindow : Window
    {
       
        public NewWindow()
        {
            InitializeComponent();

            this.ResizeMode = ResizeMode.CanMinimize;
            foreach (JobMessage currentElement in MainWindow.getListOfJobs())
            {
                TaskPlayerControl tpc = new TaskPlayerControl(Scheduler.Scheduler.getInstance().Schedule(currentElement));
                tpc.Tag = currentElement;
                tpc.jobName.Content = currentElement.Name;
                stackPanel.Children.Add(tpc);

            }
        }
    }
}