#define DEBUG

using System;
using System.Diagnostics;
using System.IO;
using Contoso.Forms.Demo.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    using XamarinDevice = Xamarin.Forms.Device;

    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class CrashesContentPage
    {
        public const string TextAttachmentKey = "TEXT_ATTACHMENT";
        public const string FileAttachmentKey = "FILE_ATTACHMENT";

        public CrashesContentPage()
        {
            InitializeComponent();
            if (XamarinDevice.RuntimePlatform == XamarinDevice.iOS)
            {
                Icon = "socket.png";
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            CrashesEnabledSwitchCell.On = await Crashes.IsEnabledAsync();
            CrashesEnabledSwitchCell.IsEnabled = await AppCenter.IsEnabledAsync();

            // Attachments
            if (Application.Current.Properties.TryGetValue(TextAttachmentKey, out var textAttachment) &&
                textAttachment is string text)
            {
                TextAttachmentCell.Detail = text;
            }
            if (Application.Current.Properties.TryGetValue(FileAttachmentKey, out var fileAttachment) &&
                fileAttachment is string file)
            {
                var filePicker = DependencyService.Get<IFilePicker>();
                try
                {
                    FileAttachmentCell.Detail = filePicker?.GetFileDescription(file);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Couldn't read file attachment: {0}", e.Message);
                    Application.Current.Properties.Remove(FileAttachmentKey);
                }
            }
            if (XamarinDevice.RuntimePlatform == XamarinDevice.UWP)
            {
                TextAttachmentCell.IsEnabled = false;
                FileAttachmentCell.IsEnabled = false;
            }
        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await Crashes.SetEnabledAsync(e.Value);
        }

        async void TextAttachment(object sender, EventArgs e)
        {
            var text = await TextAttachmentView.Show(Navigation);
            ((TextCell)sender).Detail = text;
            Application.Current.Properties[TextAttachmentKey] = text;
            await Application.Current.SavePropertiesAsync();
        }

        async void FileAttachment(object sender, EventArgs e)
        {
            var filePicker = DependencyService.Get<IFilePicker>();
            if (filePicker == null)
            {
                Debug.WriteLine("File attachment isn't implemented");
                return;
            }
            var file = await filePicker.PickFile();
            ((TextCell)sender).Detail = file;
            Application.Current.Properties[FileAttachmentKey] = file;
            await Application.Current.SavePropertiesAsync();
        }

        void TestCrash(object sender, EventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        void DivideByZero(object sender, EventArgs e)
        {
            /* This is supposed to cause a crash, so we don't care that the variable 'x' is never used */
#pragma warning disable CS0219
            int x = (42 / int.Parse("0"));
#pragma warning restore CS0219
        }

        private void CrashWithNullReferenceException(object sender, EventArgs e)
        {
            TriggerNullReferenceException();
        }

        private void TriggerNullReferenceException()
        {
            string[] values = { "one", null, "two" };
            for (int ctr = 0; ctr <= values.GetUpperBound(0); ctr++)
                Debug.WriteLine("{0}{1}", values[ctr].Trim(),
                              ctr == values.GetUpperBound(0) ? "" : ", ");
            Debug.WriteLine("");
        }

        private void CrashWithAggregateException(object sender, EventArgs e)
        {
            throw PrepareException();
        }

        private static Exception PrepareException()
        {
            try
            {
                throw new AggregateException(SendHttp(), new ArgumentException("Invalid parameter", ValidateLength()));
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Exception SendHttp()
        {
            try
            {
                throw new IOException("Network down");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Exception ValidateLength()
        {
            try
            {
                throw new ArgumentOutOfRangeException(null, "It's over 9000!");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public async void CrashAsync(object sender, EventArgs e)
        {
            await FakeService.DoStuffInBackground();
        }
    }
}
