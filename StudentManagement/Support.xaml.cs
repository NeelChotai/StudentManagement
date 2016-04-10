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
            MailAddress fromAddress = new MailAddress("studentmanagementerrors@gmail.com", "Student Management Error");
            MailAddress toAddress = new MailAddress("neel@neelchotai.com", "Neel Chotai");
            const string fromPassword = "6)P.xYC^9P&*pc.*";

            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (MailMessage message = new MailMessage(fromAddress, toAddress)
            {
                Subject = textBox.Text,
                Body = textBlock.Text
            })
            {
                try
                {
                    smtp.Send(message);
                }
                catch (SmtpException)
                {
                    MessageBox.Show("There was an issue sending your email. Please wait and try again in a moment.", 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
                catch (Exception)
                {
                    MessageBox.Show("There was an issue sending your email. Have you made sure you are online?", 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
            }
        }
    }
}
