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
    /// <summary>
    /// Interaction logic for NewWindow.xaml
    /// </summary>
    public partial class NewWindow : Window
    {
        static List<TaskPlayerControl> tpcList = new List<TaskPlayerControl>();
        //Moze ovdje i List<job> gdje ces cuvati sve kreirane jobove (tipa BlurImageJob, SharpenImageJob...)
        public NewWindow()
        {
            InitializeComponent();
            //ovdje kreirati factory objekat

            
            foreach (JobCreationElements currentElement in MainWindow.getListOfJobs()) { 
                
                TaskPlayerControl tpc=new TaskPlayerControl();
               
                    //ovdje mozes dodavati u listu pomocu factory objekta (npr. factoryObject.createJob(JobCreationElements type))
                    tpc.Tag = JobFactory.createJob(currentElement); //current element ce biti taj novi dodati Job
                    tpc.jobName.Content = currentElement.Name;
                    stackPanel.Children.Add(tpc);
                    tpcList.Add(tpc);
                
            }
        }
       
        private void TaskPlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
           // Job job = new Job();
        }
        



    }
}
