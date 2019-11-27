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
        public void WindowClosing(object sender, CancelEventArgs e) //called when window is closed
        {
            Login login = new Login();
            this.Hide();
            login.Show();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string[] username = Title.Split('\''); //gets the username from the window tityle
            if (passwordBox.Password != passwordBox1.Password) //checks if the password was entered correctly twice
            {
                if (passwordBox.Password.Length < 6) //if the password is 5 characters or less, the user must select a longer password due to both security and the reset window will show if not
                {
                    MessageBox.Show("Please enter a password longer than 5 characters.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error); //shows error
                }
                else
                {
                    database.UpdatePassword(username[0], security.Encrypt(passwordBox.Password)); //updates password with input
                }
            }
            this.Close(); //closes window
        }
    }
}
