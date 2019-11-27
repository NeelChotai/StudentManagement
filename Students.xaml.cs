using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.IO;
using Microsoft.Win32;
using System.Drawing;

namespace StudentManagement
{
    /// <summary>
    /// Interaction logic for Students.xaml
    /// </summary>
    public partial class Students : Window
    {
        private Database database = new Database();
        public Students()
        {
            InitializeComponent();
            textBox1.IsReadOnly = true;
            textBox2.IsReadOnly = true;
            textBox3.IsReadOnly = true;
            textBox4.IsReadOnly = true;
            textBox5.IsReadOnly = true;
            textBox6.IsReadOnly = true;
            button1.IsEnabled = false;
            button3.IsEnabled = false;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            listView.ItemsSource = null; 
            listView.Items.Clear(); //wipes the listView when a new item is searched for
            var name = new CultureInfo("en-GB", false); //searches for the student in title case
            var students = database.GetStudent(name.TextInfo.ToTitleCase(textBox.Text)); //calls the get student method
            listView.ItemsSource = students; //displays the given information in the listView
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(string.Format("All of the student's details will be removed from the database.\nAre you sure you want to proceed and delete student {0}?", textBox1.Text),
                "Warning",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                database.RemoveStudent(Convert.ToByte(textBox6.Text));
                textBox1.Text = "Name";
                textBox2.Text = "Form";
                textBox3.Text = "Date of Birth";
                textBox4.Text = "Emergency Contact Name";
                textBox5.Text = "Emergency Contact Number";
                textBox6.Text = "Student ID";
                button1.IsEnabled = false;
                button3.IsEnabled = false;
                listView.Items.Refresh();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = listView.SelectedItem as Student; //creates a new student object
            var studentPath = Path.Combine(Environment.CurrentDirectory, "images", string.Format("{0}.png", selectedStudent.ID)); //gets student photo according to the ID given
            OpenFileDialog selectImage = new OpenFileDialog(); //creates a new file dialog result
            selectImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); //sets the default directory to photos
            selectImage.Filter = "Image Files (*.png;*.jpeg,*.jpg)|*.png;*.jpeg;.jpg"; //filters the selectable files according to the images supported
            bool? result = selectImage.ShowDialog(); //checks if the user cancels the file dialog
            if (result.HasValue && (bool)result) //checks if the bool is null and if not, casts the result to a bool type
            {
                var img = new Bitmap(selectImage.FileName); //creates a new bitmap according to the filepath selected
                if (File.Exists(studentPath)) //checks if directory exists
                {
                    try
                    {
                        File.Delete(studentPath); //deletes the existing file
                        img.Save(studentPath, System.Drawing.Imaging.ImageFormat.Png); //saves the chosen file in a png format
                        var studentImage = new BitmapImage(); //creates a new image object
                        var stream = File.OpenRead(studentPath); //creates a new file stream object with the selected photo file
                        studentImage.BeginInit();
                        studentImage.CacheOption = BitmapCacheOption.OnLoad; //saves the chosen file in the cache
                        studentImage.StreamSource = stream; //sets the source to the stream and opens the stream
                        studentImage.EndInit();
                        stream.Close(); //closes the stream
                        stream.Dispose(); //disposes the stream's used resources
                        image.Source = studentImage; //sets the image to the new chosen file
                    }
                    catch (IOException) //catches if the file is being used by another process
                    {
                        MessageBox.Show("File is being used by another proccess. Please close it and try again.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error); //shows the user if it is
                    }
                }
                else
                {
                    img.Save(studentPath, System.Drawing.Imaging.ImageFormat.Png); //if the file does not exist, the program saves the new photo file
                }
                img.Dispose(); //diposes the image once done
            }
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            tabControl.SelectedIndex = 1; //changes to second tab page
            var selectedStudent = listView.SelectedItem as Student; //creates a new student object from the selected listView object
            var studentInformation = database.GetStudentByID(selectedStudent.ID); //gets all the information for a student
            var path = Path.Combine(Environment.CurrentDirectory, "images", string.Format("{0}.png", selectedStudent.ID)); //finds the filepath for the student based on their ID
            var studentImage = new BitmapImage(); //creates a new student bitmap object
            try
            {
                var stream = File.OpenRead(path); //opens a new file stream on the given file path
                studentImage.BeginInit();
                studentImage.CacheOption = BitmapCacheOption.OnLoad; //stores the photo in the cache so the stream can be closed
                studentImage.StreamSource = stream; //creates the stream
                studentImage.EndInit();
                stream.Close(); //closes the stream
                stream.Dispose(); //disposes of the stream's resources
                image.Source = studentImage; //sets the image of the student to the given image
                
            }
            catch (FileNotFoundException) //in the case that the photo is not found, this displays the default not found photo
            {
                var notFoundPath = Path.Combine(Environment.CurrentDirectory, "images", "not-found.png"); //local not found photo
                var stream = File.OpenRead(notFoundPath); //opens a new file stream on the path of the not found photo
                studentImage.BeginInit();
                studentImage.CacheOption = BitmapCacheOption.OnLoad; //stores the photo in the cache so the stream can be closed
                studentImage.StreamSource = stream; //creates the stream
                studentImage.EndInit();
                stream.Close(); //closes the stream
                stream.Dispose(); //disposes of the stream's resources
                image.Source = studentImage; //sets the image of the student to the not found image
            }
            textBox6.Text = studentInformation.ID.ToString(); //sets the textboxes to the given information
            textBox1.Text = studentInformation.Name;
            textBox2.Text = studentInformation.Form;
            textBox3.Text = studentInformation.DateOfBirth;
            textBox4.Text = studentInformation.EmergencyName;
            textBox5.Text = studentInformation.EmergencyNumber;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (textBox6.Text == "Student ID") //checks if a student has been chosen
            {
                MessageBox.Show("Please select a student first.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error); //shows error if not
            }
            else
            {
                button1.IsEnabled = true; //enables the upload photo and remove student buttons
                button3.IsEnabled = true;
            }
        }
    }
}
