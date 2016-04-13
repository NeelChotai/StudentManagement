using System.Windows;
using System.Net;
using System.Net.Mail;
using System;

namespace StudentManagement
{
    /// <summary>
    /// Interaction logic for Support.xaml
    /// </summary>
    public partial class Support : Window
    {
        public Support()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MailAddress fromAddress = new MailAddress("studentmanagementerrors@gmail.com", "Student Management Error"); //email address to send to and where the email is sent fromAddress
            const string fromPassword = "6)P.xYC^9P&*pc.*"; //email password

            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword) //sets credentials to the student management credentials
            }; //creates a new smtp client for gmail
            using (MailMessage message = new MailMessage(fromAddress, fromAddress)
            {
                Subject = textBox.Text,
                Body = textBlock.Text, //creates message from inputs
            })
            {
                try
                {
                    smtp.Send(message); //tries to send message
                }
                catch (SmtpException)
                {
                    MessageBox.Show("There was an issue sending your email. Please wait and try again in a moment.", 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error); //catches a server error and asks the user to wait before sending again
                }
                catch (Exception)
                {
                    MessageBox.Show("There was an issue sending your email. Have you made sure you are online?", 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error); //catches an exception which is most likely to be an offline exception
                }
            }
        }
    }
}
