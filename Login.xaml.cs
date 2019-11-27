using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StudentManagement
{
    public partial class Login : Window
    {
        private Database database = new Database();
        private Encryption security = new Encryption();
        public Login()
        {
            InitializeComponent();
            button.IsEnabled = false;
            ServerCheck();
        }
        void ServerCheck()
        {
            database.OpenConnection();
            switch (database.Status())
            {
                case true:
                    label1.Content = "Connected";
                    label1.Foreground = Brushes.Green;
                    button.IsEnabled = true;
                    break;
                case false:
                    label1.Content = "Disconnected";
                    label1.Foreground = Brushes.Red;
                    break;
            }
            database.CloseConnection();
        }
        void UserType()
        {
            byte accessRights = database.GetAccessRights(textBox.Text); //gets access rights for username entered
            switch (accessRights)
            {
                case (byte)Database.Permissions.Admin:
                case (byte)Database.Permissions.AlphaAdmin:
                    AdminMain adminMain = new AdminMain() { Title = string.Format("{0}'s Panel", textBox.Text) }; //in the case that an administrator or alpha administrator wishes to access the program, this will allow the administration window to be shown
                    this.Hide();
                    adminMain.Show();
                    this.Close();
                    break;

                case (byte)Database.Permissions.User:
                    Main main = new Main() { Title = string.Format("{0}'s Panel", textBox.Text) }; //otherwise the regular user window is shown
                    this.Hide();
                    main.Show();
                    this.Close();
                    break;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
                MessageBox.Show("Please enter a username.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error); //checks if there is a username entered, reports error if not

            else if (string.IsNullOrWhiteSpace(passwordBox.Password))
                MessageBox.Show("Please enter a password.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error); //checks if there is a password entered, reports error if not

            else if (security.Verify(textBox.Text, passwordBox.Password))
            {
                if (passwordBox.Password.Length < 6)
                {
                    ChangePassword changePassword = new ChangePassword() { Title = string.Format("{0}'s New Password", textBox.Text) };
                    this.Hide();
                    changePassword.Show();
                    this.Close(); //all reset passwords will be 5 characters long and the user, for security reasons, will have to reset their password the first time they log in - this is to make sure they do so
                }
                else
                {
                    UserType();
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password, please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error); //catches any other error
                passwordBox.Password = string.Empty;
            }
        }
    }
}
