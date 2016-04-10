using System.Windows;
using System.ComponentModel;

namespace StudentManagement
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        private Database database = new Database();
        private Encryption security = new Encryption();
        public ChangePassword()
        {
            InitializeComponent();
        }
        public void WindowClosing(object sender, CancelEventArgs e)
        {
            Login login = new Login();
            this.Hide();
            login.Show();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string[] username = Title.Split('\'');
            if (passwordBox.Password != passwordBox1.Password)
            {
                if (passwordBox.Password.Length < 6)
                {
                    MessageBox.Show("Please enter a password longer than 5 characters.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                }
                else
                {
                    database.UpdatePassword(username[0], security.Encrypt(passwordBox.Password));
                }
            }
            this.Close();
        }
    }
}
