using Newtonsoft.Json;
using OPOS_project.Scheduler;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace OPOS_project
{
    public partial class MainWindow : Window
    {

        private JobType? selectedJobType = null;
        private DateTime? selectedStartDate = null;
        private DateTime? selectedEndDate = null;
        private System.Windows.Controls.Image selectedImage = null;
        private Bitmap? selectedBitmap = null;
        private int? selectedTotalExecutionTime = null;
        public static List<JobMessage> listOfJobs = new List<JobMessage>();
        public static MessageQueue messageQueue = MessageQueue.getInstance();

        private static int MAX_NUMBER_OF_PROCESSES = 3;

        public MainWindow()
        {

            InitializeComponent();


            string bitmapPath2 = @"../../../Resources/city.png";
            this.fillWithTestData();

            // messageQueue.PublishMessage(jobMessage2);   


            this.ResizeMode = ResizeMode.CanMinimize;
            comboBoxSelectJob.Items.Clear();
            JobType[] elements = (JobType[])Enum.GetValues(typeof(JobType));

            foreach (JobType element in elements) //fing the combo box with job types
            {
                comboBoxSelectJob.Items.Add(new ComboBoxItem { Content = element.ToString(), Tag = element });
            }

        }

        private void timeControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public static List<JobMessage> getListOfJobs()
        {
            return listOfJobs;
        }

        private void ComboBox_DropDownOpened(object sender, System.EventArgs e)
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //choosing option from combo box
        {
            if (comboBoxSelectJob.SelectedItem != null)
                selectedJobType = (JobType)(((ComboBoxItem)comboBoxSelectJob.SelectedItem).Tag);
        }

        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Filter = "Image s (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    string selectedFileName = openFileDialog.FileName;
                    imagePath.Content = selectedFileName;
                    selectedBitmap = new Bitmap(selectedFileName);
                    BitmapImage bitmapImage = new BitmapImage(new Uri(selectedFileName));
                    selectedImage = new System.Windows.Controls.Image
                    {
                        Source = bitmapImage
                    };
                    testImage.Source = selectedImage.Source;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void startJobsButton_Click(object sender, RoutedEventArgs e)
        {
            //illWithTestData();

            //NewWindow newWindow = new NewWindow();

            //newWindow.Show();

            for(int i = 0; i < MAX_NUMBER_OF_PROCESSES; i++) 
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "C:\\Users\\WIN11\\Desktop\\OPOS_project\\OPOS_project\\OPOS_Consumer\\bin\\Debug\\net8.0-windows\\OPOS_Consumer.exe",
                    UseShellExecute = true
                });
            }
        }

        private DateTime buildDateAndTime(DateTime selectedDate, DateTime selectedTime)
        {
            DateTime newDateAndTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, selectedTime.Hour, selectedTime.Minute, selectedTime.Second);
            return newDateAndTime;
        }

        private Boolean CheckDateAndTimeInputs()
        {
            if (selectedStartDate != null && startTimePicker.DateTimeValue != null)
            {
                selectedStartDate = buildDateAndTime((DateTime)selectedStartDate, (DateTime)startTimePicker.DateTimeValue);
            }
            else
            {
                printMessageLabel.Content = "Job start date or start time are not selected";
                return false;
            }
            if (selectedEndDate != null && endTimePicker.DateTimeValue != null)
            {
                selectedEndDate = buildDateAndTime((DateTime)selectedEndDate, (DateTime)endTimePicker.DateTimeValue);
            }
            else
            {
                printMessageLabel.Content = "Job end date or end time are not selected";
                return false;
            }
            if ((DateTime.Now.CompareTo(selectedStartDate) > 0 || DateTime.Now.CompareTo(selectedEndDate) > 0))
            {
                printMessageLabel.Content = "Job date and time is earlier than current date and time";
                return false;
            }
            if (selectedEndDate < selectedStartDate)
            {
                printMessageLabel.Content = "End date and time can't be earlier than start date and time";
                return false;
            }
            if (totalExecutionTimePicker.DateTimeValue != null)
            {
                selectedTotalExecutionTime = (int)((DateTime)totalExecutionTimePicker.DateTimeValue).TimeOfDay.TotalSeconds;
                printMessageLabel.Content = selectedTotalExecutionTime.ToString();
            }
            else
            {
                printMessageLabel.Content = "Job execution time is not selected";
                return false;
            }
            int maxTimeSpan = 0;
            maxTimeSpan = selectedEndDate.Value.Second - selectedStartDate.Value.Second;
            if (maxTimeSpan < selectedTotalExecutionTime)
            {
                printMessageLabel.Content = "Selected execution time is too long";
            }
            return true;
        }

        public void clearEnteredElements()
        {
            comboBoxSelectJob.SelectedIndex = -1;
            imagePath.Content = "";
            startDatePicker.SelectedDate = null;
            endDatePicker.SelectedDate = null;
            startTimePicker.ClearValues();
            endTimePicker.ClearValues();
            isTimedJob.IsChecked = false;
        }

        private void addJobButton_Click(object sender, RoutedEventArgs e)
        {
            JobMessage jobMessage = null;
            if (isTimedJob.IsChecked == true)
            {

                if (CheckDateAndTimeInputs())
                {
                    if (selectedJobType != null && selectedImage != null)
                    {

                        jobMessage = new JobMessage($"Blur_1", JobType.Blur, (String)imagePath.Content,
                        selectedStartDate, selectedEndDate, selectedTotalExecutionTime);
                        messageQueue.PublishMessageToScheduled(jobMessage);
                        printMessageLabel.Content = "Job sucessfully added";
                        clearEnteredElements();


                    }
                    else return;
                }

                

            }
            else
            {
                if (selectedJobType == null)
                {
                    printMessageLabel.Content = "You need to select a job type";
                }
                else if (selectedImage == null)
                {
                    printMessageLabel.Content = "You need to select an image";
                }
                else
                {
                    jobMessage = new JobMessage($"{selectedJobType.ToString()}_{listOfJobs.Count}",
                          selectedJobType, (String)imagePath.Content);

                    printMessageLabel.Content = "Job sucessfully added";
                    comboBoxSelectJob.SelectedIndex = -1;
                    imagePath.Content = "";
                    messageQueue.PublishMessageToUnscheduled(jobMessage);

                }
            }



        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (startDatePicker != null)
            {
                selectedStartDate = startDatePicker.SelectedDate;
            }
        }




        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
             {
                 if (endDatePicker != null)
                 {
                     selectedEndDate = endDatePicker.SelectedDate;
                 }
             }
            

            private void fillWithTestData()
            {
                string bitmapPath1 = @"../../../Resources/city.png";
                string bitmapPath2 = @"../../../Resources/hamster.png";
                string bitmapPath3 = @"../../../Resources/nature.png";

                string fullPath1 =  Path.GetFullPath(bitmapPath1);
                string fullPath2 =  Path.GetFullPath(bitmapPath2);
                string fullPath3 =  Path.GetFullPath(bitmapPath3);

                JobMessage jobMessage4=new JobMessage($"Blur_1", JobType.Blur, Path.GetFullPath(bitmapPath1),
                 (DateTime.Now).AddSeconds(3), (DateTime.Now).AddMinutes(1), 10);
           
            
            Random random = new Random();
            JobMessage jobMessage1=new JobMessage($"DetectEdges"+random.NextInt64(3000), JobType.DetectEdges, fullPath2);

               JobMessage jobMessage5=new JobMessage($"Embossing_3", JobType.Embossing, Path.GetFullPath(bitmapPath3),
                    (DateTime.Now).AddSeconds(5), (DateTime.Now).AddMinutes(1), 10);
               

                JobMessage jobMessage2=new JobMessage($"Blur_" + random.NextInt64(3000), JobType.Blur, fullPath1);

               /* listOfJobs.Add(new JobMessage($"Embossing_5", JobType.Embossing, bitmapPath2,
                    (DateTime.Now).AddSeconds(7), (DateTime.Now).AddMinutes(1), 10));
               */

                JobMessage jobMessage3=new JobMessage($"Sharpen_" + random.NextInt64(3000), JobType.Sharpen, fullPath3);

            /*  listOfJobs.Add(new JobMessage($"Blur_7", JobType.Blur, bitmapPath2,
                  (DateTime.Now).AddSeconds(10), (DateTime.Now).AddSeconds(15), 30));

              listOfJobs.Add(new JobMessage($"Blur_8", JobType.Blur, bitmapPath2,
                  (DateTime.Now).AddSeconds(12), (DateTime.Now).AddSeconds(30), 8));
            */
            messageQueue.PublishMessageToUnscheduled(jobMessage1);
            messageQueue.PublishMessageToUnscheduled(jobMessage2);
            messageQueue.PublishMessageToUnscheduled(jobMessage3);
            messageQueue.PublishMessageToUnscheduled(jobMessage1);
            messageQueue.PublishMessageToUnscheduled(jobMessage2);
            messageQueue.PublishMessageToUnscheduled(jobMessage3);
            messageQueue.PublishMessageToUnscheduled(jobMessage1);
            messageQueue.PublishMessageToUnscheduled(jobMessage2);
            messageQueue.PublishMessageToUnscheduled(jobMessage3);
            messageQueue.PublishMessageToUnscheduled(jobMessage1);
            messageQueue.PublishMessageToUnscheduled(jobMessage2);
            messageQueue.PublishMessageToUnscheduled(jobMessage3);

            messageQueue.PublishMessageToScheduled(jobMessage4);
            messageQueue.PublishMessageToScheduled(jobMessage5);


            //messageQueue.Dispose();


        }

        private void isTimedJob_Checked(object sender, RoutedEventArgs e)
        {
            blurRectangle.Visibility = Visibility.Collapsed;
        }

        private void isTimedJob_Unchecked(object sender, RoutedEventArgs e)
        {
            blurRectangle.Visibility = Visibility.Visible;
        }
    }
    }
