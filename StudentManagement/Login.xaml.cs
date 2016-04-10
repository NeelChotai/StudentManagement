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
            passwordBox.KeyDown += new KeyEventHandler(EnterClicked);
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
        void userType()
        {
            byte accessRights = database.GetAccessRights(textBox.Text);
            switch (accessRights)
            {
                case (byte)Database.Permissions.Admin:
                case (byte)Database.Permissions.AlphaAdmin:
                    AdminMain adminMain = new AdminMain() { Title = string.Format("{0}'s Control Panel", textBox.Text) };
                    this.Hide();
                    adminMain.Show();
                    this.Close();
                    break;

                case (byte)Database.Permissions.User:
                    Main main = new Main() { Title = string.Format("{0}'s Control Panel", textBox.Text) };
                    this.Hide();
                    main.Show();
                    this.Close();
                    break;
            }
        }
        void EnterClicked(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                userType();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
                MessageBox.Show("Please enter a username.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

            else if (string.IsNullOrWhiteSpace(passwordBox.Password))
                MessageBox.Show("Please enter a password.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

            else if (security.Verify(textBox.Text, passwordBox.Password))
            {
                if (passwordBox.Password.Length < 6)
                {
                    ChangePassword changePassword = new ChangePassword() { Title = string.Format("{0}'s New Password", textBox.Text) };
                    this.Hide();
                    changePassword.Show();
                    this.Close();
                }
                else
                {
                    userType();
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password, please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                passwordBox.Password = string.Empty;
            }
        }
    }
}
