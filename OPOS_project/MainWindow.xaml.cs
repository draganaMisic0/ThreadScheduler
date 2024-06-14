using OPOS_project.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();



        private JobType? selectedJobType = null;
        private DateTime? selectedDate= null;
        private Image? selectedImage = null;
        public static List<JobCreationElements> listOfJobs = new List<JobCreationElements>();
       // public static TextBox ? testTextBox { get; }
        public MainWindow()
        {
            InitializeComponent();
            comboBoxSelectJob.Items.Clear();
            JobType[] elements = (JobType[])Enum.GetValues(typeof(JobType));
            foreach (JobType element in elements)
            {
                comboBoxSelectJob.Items.Add(new ComboBoxItem { Content = element.ToString(), Tag = element });
            }

            AllocConsole();
            Console.WriteLine("Console window allocated.");

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
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
           
          
            if (datePicker != null)
            {
                selectedDate = datePicker.SelectedDate;
                testTextBox.AppendText($"Selected Date: {selectedDate?.ToString("d")}");
            }
        }
        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void timeControl_Loaded(object sender, RoutedEventArgs e)
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
                        BitmapImage bitmapImage = new BitmapImage(new Uri(selectedFileName));
                        selectedImage = new Image();
                        selectedImage.Source = bitmapImage;
                    testImage.Source = selectedImage.Source;


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

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            testTextBox.AppendText(timeControl.printSelectedTimeString());
        }

        private void startJobsButton_Click(object sender, RoutedEventArgs e)
        {
            NewWindow newWindow = new NewWindow();
           
            newWindow.Show();
        }
        private DateTime buildDateAndTime(DateTime selectedDate, DateTime selectedTime)
        {

            DateTime startDateAndTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, selectedTime.Hour, selectedTime.Minute, selectedTime.Second);
            return startDateAndTime;

        }
        private void addJobButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDate != null && timeControl.DateTimeValue != null)
            { selectedDate = buildDateAndTime((DateTime)selectedDate, (DateTime)timeControl.DateTimeValue); }
            else
            {
                return;
            }

            if(selectedJobType!=null && selectedDate!=null && selectedImage!=null)
                 listOfJobs.Add(new JobCreationElements($"{selectedJobType.ToString()}_{listOfJobs.Count}",selectedJobType, selectedDate, selectedImage));

        }
    }
}
