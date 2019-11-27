using System.Windows;

namespace StudentManagement
{
    public partial class AdminMain : Window
    {
        public AdminMain()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Students students = new Students(); //new instance of students
            students.Show();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Users users = new Users(); //new instance of users
            users.Show();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Support support = new Support(); //new instance of support
            support.Show();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login(); //new instance of login when the user wishes to log out
            this.Hide();
            login.Show();
            this.Close();
        }
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            string[] username = Title.Split('\''); //gets username from the title
            ChangePassword changePassword = new ChangePassword() { Title = Title = string.Format("{0}'s New Password", username[0]) }; //creates a new instance of the change password window
            this.Hide();
            changePassword.Show();
            this.Close(); 
        }
    }
}
