using Microsoft.AppCenter.Crashes;
using System.Windows;

namespace Contoso.WPF.Demo
{
    public partial class UserConfirmationDialog : Window
    {
        public UserConfirmationDialog()
        {
            InitializeComponent();
        }

        private void DontSendButton_Click(object sender, RoutedEventArgs e)
        {
            ClickResult = UserConfirmation.DontSend;
            DialogResult = true;
        }

        private void AlwaysSendButton_Click(object sender, RoutedEventArgs e)
        {
            ClickResult = UserConfirmation.AlwaysSend;
            DialogResult = true;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ClickResult = UserConfirmation.Send;
            DialogResult = true;
        }

        public UserConfirmation ClickResult { get; private set; }
    }
}
