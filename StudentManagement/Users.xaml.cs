using System;
using System.Linq;
using System.Windows;

namespace StudentManagement
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Window
    {
        private Database database = new Database();
        private Encryption security = new Encryption();
        public Users()
        {
            InitializeComponent();
            button.Visibility = Visibility.Hidden;
            button1.Visibility = Visibility.Hidden;
            button2.Visibility = Visibility.Hidden;
            button3.Visibility = Visibility.Hidden;
            textBox.IsEnabled = false;
            comboBox.IsEnabled = false;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) || string.IsNullOrWhiteSpace(comboBox.Text))
            {
                MessageBox.Show("Please enter a valid username or access rights.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            else if (database.GetAccessRights(textBox.Text) == (byte)comboBox.SelectedIndex)
            {
                MessageBox.Show("User already has this level of access.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else if (textBox.Text.Contains('\'') || textBox.Text.Contains(' '))
            {
                MessageBox.Show("Invalid character in username. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                database.UpdateAccess(textBox.Text, (byte)comboBox.SelectedIndex);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) || string.IsNullOrWhiteSpace(comboBox.Text)) //checks if anything is empty
            {
                MessageBox.Show("Please enter a valid username or access rights.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error); //shows the user an error if there is anything empty
            }
            else if (textBox.Text.Contains('\'') || textBox.Text.Contains(' ')) //checks for invalid characters
            {
                MessageBox.Show("Invalid character in username. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error); //notifies the user that there are invalid characters
            }
            else
            {
                string password = RandomString(); //generates a new random string
                database.CreateNewUser(textBox.Text, security.Encrypt(password), (byte)comboBox.SelectedIndex); //creates a new user with the username in the textBox, an encrypted passwords and access rights at the index they chose
                MessageBox.Show(string.Format("User has been created. Their password is: {0}\nThe user will have to change this password on their initial login.", password),
                    "New Password",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information); //shows the user the new password
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                MessageBox.Show("Please enter for a valid username.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else if (database.GetAccessRights(textBox.Text) == (byte)Database.Permissions.AlphaAdmin)
            {
                MessageBox.Show("Cannot delete this user.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(string.Format("All of the user's details will be removed from the database.\nAre you sure you want to proceed and delete user {0}?", textBox.Text),
                    "Warning",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK)
                {
                    database.RemoveUser(textBox.Text);
                }
                textBox.Text = "Username";
                comboBox.Text = string.Empty;
            }
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (button.Visibility == Visibility.Visible)
            {
                button.Visibility = Visibility.Hidden;
                button2.Visibility = Visibility.Hidden;
                button3.Visibility = Visibility.Hidden;
            }
            textBox.IsEnabled = true;
            comboBox.IsEnabled = true;
            button1.Visibility = Visibility.Visible;
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            if (button1.Visibility == Visibility.Visible)
            {
                button1.Visibility = Visibility.Hidden;
            }
            textBox.IsEnabled = true;
            comboBox.IsEnabled = true;
            button.Visibility = Visibility.Visible;
            button2.Visibility = Visibility.Visible;
            button3.Visibility = Visibility.Visible;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                MessageBox.Show("Please enter for a valid username.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                string password = RandomString();
                try
                {
                    database.ResetPassword(textBox.Text, security.Encrypt(password));
                }
                finally
                {
                    MessageBox.Show(string.Format("User's new password is: {0}\nThe user will have to change this password on their initial login.", password),
                    "New Password",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                }
            }
        }
        private static string RandomString()
        {
            Random generator = new Random();
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, 5).Select(x => input[generator.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }
    }
}
