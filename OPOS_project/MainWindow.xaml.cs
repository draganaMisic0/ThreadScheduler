using OPOS_project.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.IO;

namespace OPOS_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();



        private JobType? selectedJobType = null;
        private DateTime? selectedStartDate= null;
        private DateTime? selectedEndDate = null;
        private System.Windows.Controls.Image selectedImage = null;
        private Bitmap? selectedBitmap = null;
        private int? selectedTotalExecutionTime = null;
        public static List<JobCreationElements> listOfJobs = new List<JobCreationElements>();
       // public static TextBox ? testTextBox { get; }
        public MainWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
            comboBoxSelectJob.Items.Clear();
            JobType[] elements = (JobType[])Enum.GetValues(typeof(JobType));
            foreach (JobType element in elements)
            {
                comboBoxSelectJob.Items.Add(new ComboBoxItem { Content = element.ToString(), Tag = element });
            }

            AllocConsole();
            Console.WriteLine("Console window allocated.");

        }
        private void timeControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Your event handler logic here
        }
        public static List<JobCreationElements> getListOfJobs() {
            
            return listOfJobs;
        }
        
        private void ComboBox_SelectionChanged(object sender, EventArgs e)
        {
            // comboBoxSelectJob.Visibility = Visibility.Visible;
        }
      
        private void ComboBox_DropDownOpened(object sender, System.EventArgs e) //vjerovatno ni ne treba
        {
            
            

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxSelectJob.SelectedItem != null)
                selectedJobType = (JobType)(((ComboBoxItem)comboBoxSelectJob.SelectedItem).Tag) ;
            testTextBox.AppendText(selectedJobType.ToString());
        }
      
        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
           
                // Create OpenFileDialog
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

                // Set filter for file extension and default file extension
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*";

                // Display OpenFileDialog by calling ShowDialog method
                bool? result = openFileDialog.ShowDialog();

                // Check if a file was selected
                if (result == true)
                {
                    try
                    {
                        // Get the selected file name and display in a TextBox
                        string selectedFileName = openFileDialog.FileName;
                        imagePath.Content=selectedFileName;
                        selectedBitmap=new Bitmap(selectedFileName);
                        BitmapImage bitmapImage = new BitmapImage(new Uri(selectedFileName));
                    selectedImage = new System.Windows.Controls.Image
                    {
                        Source = bitmapImage
                    };
                    testImage.Source = selectedImage.Source;
                    selectedBitmap.Save("C:\\Users\\WIN11\\Desktop\\OPOS_project\\OPOS_project\\OPOS_project\\file\\image.png", ImageFormat.Png);

                    // Perform your upload action here, such as saving the file or sending it to a server
                    // For example:
                    // MessageBox.Show("Selected file: " + selectedFileName);
                    // You can also assign the file path to an Image control's source property to display the image:
                    // imageControl.Source = new BitmapImage(new Uri(selectedFileName));
                }
                catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            

        }

     
        private void startJobsButton_Click(object sender, RoutedEventArgs e)
        {
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
            maxTimeSpan=selectedEndDate.Value.Second - selectedStartDate.Value.Second;
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

                        listOfJobs.Add(new JobCreationElements($"{selectedJobType.ToString()}_{listOfJobs.Count}",
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
                else if(selectedImage==null)
                {
                    printMessageLabel.Content = "You need to select an image";
                }
                else
                {
                    listOfJobs.Add(new JobCreationElements($"{selectedJobType.ToString()}_{listOfJobs.Count}",
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
                testTextBox.AppendText($"Selected Date: {selectedStartDate?.ToString("d")}");
            }
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            if (endDatePicker != null)
            {
                selectedEndDate = endDatePicker.SelectedDate;
                testTextBox.AppendText($"Selected Date: {selectedEndDate?.ToString("d")}");
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
    }
}
