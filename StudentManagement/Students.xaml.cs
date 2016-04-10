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
            listView.Items.Clear();
            var name = new CultureInfo("en-GB", false);
            var students = database.GetStudent(name.TextInfo.ToTitleCase(textBox.Text));
            listView.ItemsSource = students;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(string.Format("All of the student's details will be removed from the database.\nAre you sure you want to proceed and delete student {0}?", textBox1.Text),
                "Warning",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                database.RemoveStudent(Convert.ToInt32(textBox6.Text));
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
            var selectedStudent = listView.SelectedItem as Student;
            var studentPath = Path.Combine(Environment.CurrentDirectory, "images", string.Format("{0}.png", selectedStudent.ID));
            OpenFileDialog selectImage = new OpenFileDialog();
            selectImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            selectImage.Filter = "Image Files (*.png;*.jpeg,*.jpg)|*.png;*.jpeg;.jpg";
            bool? result = selectImage.ShowDialog();
            if (result.HasValue && (bool)result)
            {
                var img = new Bitmap(selectImage.FileName);
                if (File.Exists(studentPath))
                {
                    try
                    {
                        File.Delete(studentPath);
                        img.Save(studentPath, System.Drawing.Imaging.ImageFormat.Png);
                        var studentImage = new BitmapImage();
                        var stream = File.OpenRead(studentPath);
                        studentImage.BeginInit();
                        studentImage.CacheOption = BitmapCacheOption.OnLoad;
                        studentImage.StreamSource = stream;
                        studentImage.EndInit();
                        stream.Close();
                        stream.Dispose();
                        image.Source = studentImage;
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("File is being used by another proccess. Please close it and try again.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    img.Save(studentPath, System.Drawing.Imaging.ImageFormat.Png);
                }
                img.Dispose();
            }
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            tabControl.SelectedIndex = 1;
            var selectedStudent = listView.SelectedItem as Student;
            var studentInformation = database.GetStudentByID(selectedStudent.ID);
            var path = Path.Combine(Environment.CurrentDirectory, "images", string.Format("{0}.png", selectedStudent.ID));
            var studentImage = new BitmapImage();
            try
            {
                var stream = File.OpenRead(path);
                studentImage.BeginInit();
                studentImage.CacheOption = BitmapCacheOption.OnLoad;
                studentImage.StreamSource = stream;
                studentImage.EndInit();
                stream.Close();
                stream.Dispose();
                image.Source = studentImage;
                
            }
            catch (FileNotFoundException)
            {
                var notFoundPath = Path.Combine(Environment.CurrentDirectory, "images", "not-found.png");
                var stream = File.OpenRead(notFoundPath);
                studentImage.BeginInit();
                studentImage.CacheOption = BitmapCacheOption.OnLoad;
                studentImage.StreamSource = stream;
                studentImage.EndInit();
                stream.Close();
                stream.Dispose();
                image.Source = studentImage;
            }
            textBox6.Text = studentInformation.ID.ToString();
            textBox1.Text = studentInformation.Name;
            textBox2.Text = studentInformation.Form;
            textBox3.Text = studentInformation.DateOfBirth;
            textBox4.Text = studentInformation.emergencyName;
            textBox5.Text = studentInformation.emergencyNumber;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (textBox6.Text == "Student ID")
            {
                MessageBox.Show("Please select a student first.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
            else
            {
                button1.IsEnabled = true;
                button3.IsEnabled = true;
            }
        }
    }
}
