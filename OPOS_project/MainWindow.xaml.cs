using OPOS_project.Scheduler;
using System.Drawing;
using System.Runtime.InteropServices;
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

        public MainWindow()
        {
           
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
            comboBoxSelectJob.Items.Clear();
            JobType[] elements = (JobType[])Enum.GetValues(typeof(JobType));
            
            foreach (JobType element in elements) //filling the combo box with job types
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
            fillWithTestData();

            NewWindow newWindow = new NewWindow();

            newWindow.Show();
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
            if (isTimedJob.IsChecked == true)
            {
                if (CheckDateAndTimeInputs())
                {
                    if (selectedJobType != null && selectedImage != null)

                        listOfJobs.Add(new JobMessage($"{selectedJobType.ToString()}_{listOfJobs.Count}",
                        selectedJobType, selectedBitmap, selectedStartDate, selectedEndDate, selectedTotalExecutionTime));
                    printMessageLabel.Content = "Job sucessfully added";
                    clearEnteredElements();
                }
                else return;
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
                    listOfJobs.Add(new JobMessage($"{selectedJobType.ToString()}_{listOfJobs.Count}",
                        selectedJobType, selectedBitmap));
                    printMessageLabel.Content = "Job sucessfully added";
                    comboBoxSelectJob.SelectedIndex = -1;
                    imagePath.Content = "";
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

        private void checkBoxIsTimedJob_Checked(object sender, RoutedEventArgs e)
        {
            blurRectangle.Visibility = Visibility.Collapsed;
        }

        private void CheckBoxIsTimedJob_Unchecked(object sender, RoutedEventArgs e)
        {
            blurRectangle.Visibility = Visibility.Visible;
        }

        private void fillWithTestData()
        {
            string bitmapPath1 = @"../../../Resources/city.png";
            string bitmapPath2 = @"../../../Resources/hamster.png";
            string bitmapPath3 = @"../../../Resources/nature.png";

            listOfJobs.Add(new JobMessage($"Blur_1", JobType.Blur, new Bitmap(bitmapPath1),
                (DateTime.Now).AddSeconds(3),(DateTime.Now).AddMinutes(1),10));

            listOfJobs.Add(new JobMessage($"DetectEdges_2",JobType.DetectEdges, new Bitmap(bitmapPath2)));

            listOfJobs.Add(new JobMessage($"Embossing_3", JobType.Embossing, new Bitmap(bitmapPath3),
                (DateTime.Now).AddSeconds(5), (DateTime.Now).AddMinutes(1), 10));

            listOfJobs.Add(new JobMessage($"DetectEdges_4",JobType.DetectEdges, new Bitmap(bitmapPath1)));

            listOfJobs.Add(new JobMessage($"Embossing_5", JobType.Embossing, new Bitmap(bitmapPath2),
                (DateTime.Now).AddSeconds(7), (DateTime.Now).AddMinutes(1), 10));

            listOfJobs.Add(new JobMessage($"Sharpen_6",JobType.Sharpen, new Bitmap(bitmapPath1)));

            listOfJobs.Add(new JobMessage($"Blur_7", JobType.Blur, new Bitmap(bitmapPath2),
                (DateTime.Now).AddSeconds(10), (DateTime.Now).AddSeconds(15), 30));

            listOfJobs.Add(new JobMessage($"Blur_8", JobType.Blur, new Bitmap(bitmapPath2),
                (DateTime.Now).AddSeconds(12), (DateTime.Now).AddSeconds(30), 8));


        }
    }
}