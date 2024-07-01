using OPOS_project.Scheduler;
using OPOS_project.Specific_jobs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
using System.Windows.Shapes;

namespace OPOS_project
{
  
    public partial class NewWindow : Window
    {
        //static List<TaskPlayerControl> tpcList = new List<TaskPlayerControl>();

        public NewWindow()
        {
            InitializeComponent();

            this.ResizeMode = ResizeMode.CanMinimize;
            foreach (JobCreationElements currentElement in MainWindow.getListOfJobs())
            { 
                TaskPlayerControl tpc = new TaskPlayerControl(Scheduler.Scheduler.getInstance().Schedule(currentElement));
                tpc.Tag = currentElement;
                tpc.jobName.Content = currentElement.Name;
                stackPanel.Children.Add(tpc);
                //tpcList.Add(tpc);
            }
        }
    }
}
