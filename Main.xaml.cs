using System.Windows;

namespace StudentManagement
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Students students = new Students();
            this.Hide();
            students.Show();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Support support = new Support();
            support.Show();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            this.Hide();
            login.Show();
            this.Close();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            string[] username = Title.Split('\'');
            ChangePassword changePassword = new ChangePassword() { Title = Title = string.Format("{0}'s New Password", username[0]) };
            this.Hide();
            changePassword.Show();
            this.Close();
        }
    }
}
