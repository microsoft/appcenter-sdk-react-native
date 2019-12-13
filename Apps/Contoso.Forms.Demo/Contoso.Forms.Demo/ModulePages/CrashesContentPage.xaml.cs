// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Contoso.Forms.Demo.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.Forms.Demo
{
    using XamarinDevice = Xamarin.Forms.Device;

    public class NonSerializableException : Exception
    {
    }

    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class CrashesContentPage
    {
        public const string TextAttachmentKey = "TEXT_ATTACHMENT";

        public const string FileAttachmentKey = "FILE_ATTACHMENT";

        List<Property> Properties;

        public CrashesContentPage()
        {
            InitializeComponent();
            Properties = new List<Property>();
            NumPropertiesLabel.Text = Properties.Count.ToString();
            if (XamarinDevice.RuntimePlatform == XamarinDevice.iOS)
            {
                Icon = "socket.png";
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            RefreshEnabled();
            var hasLowMemoryWarning = await Crashes.HasReceivedMemoryWarningInLastSessionAsync();
            MemoryWarningLabel.Text = hasLowMemoryWarning ? "Yes" : "No";

            // Attachments
            if (Application.Current.Properties.TryGetValue(TextAttachmentKey, out var textAttachment) &&
                textAttachment is string text)
            {
                TextAttachmentContent.Text = text;
            }
            if (Application.Current.Properties.TryGetValue(FileAttachmentKey, out var fileAttachment) &&
                fileAttachment is string file)
            {
                try
                {
                    BinaryAttachmentFilePathLabel.Text = file;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Couldn't read file attachment: {0}", e.Message);
                    Application.Current.Properties.Remove(FileAttachmentKey);
                }
            }
        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await Crashes.SetEnabledAsync(e.Value);
            RefreshEnabled();
        }

        async void TextAttachment(object sender, EventArgs e)
        {
            var text = await TextAttachmentView.Show(Navigation);
            TextAttachmentContent.Text = text;
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
            var filePath = await filePicker.PickFile();
            BinaryAttachmentFilePathLabel.Text = filePath;
            Application.Current.Properties[FileAttachmentKey] = filePath;
            await Application.Current.SavePropertiesAsync();
        }

        async void AddProperty(object sender, EventArgs e)
        {
            var addPage = new AddPropertyContentPage();
            addPage.PropertyAdded += (Property property) =>
            {
                if (property.Name == null || Properties.Any(i => i.Name == property.Name))
                {
                    return;
                }
                Properties.Add(property);
                RefreshPropCount();
            };
            await Navigation.PushModalAsync(addPage);
        }

        async void PropertiesCellTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PropertiesContentPage(Properties));
        }

        async void RefreshEnabled()
        {
            CrashesEnabledSwitchCell.On = await Crashes.IsEnabledAsync();
            CrashesEnabledSwitchCell.IsEnabled = await AppCenter.IsEnabledAsync();
        }

        void RefreshPropCount()
        {
            NumPropertiesLabel.Text = Properties.Count.ToString();
        }

        void HandleOrThrow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e) when (HandleExceptionsSwitchCell.On)
            {
                TrackException(e);
            }
        }

        void TestException(object sender, EventArgs e)
        {
            HandleOrThrow(() => Crashes.GenerateTestCrash());
        }

        void DivideByZero(object sender, EventArgs e)
        {
            /* This is supposed to cause a crash, so we don't care that the variable 'x' is never used */
#pragma warning disable CS0219
            HandleOrThrow(() => (42 / int.Parse("0")).ToString());
#pragma warning restore CS0219
        }

        void CatchNullReferenceException(object sender, EventArgs e)
        {
            try
            {
                TriggerNullReferenceException();
            }
            catch (NullReferenceException)
            {
                Debug.WriteLine("null reference exception");
            }
        }

        void NullReferenceException(object sender, EventArgs e)
        {
            HandleOrThrow(() => TriggerNullReferenceException());
        }

        void TriggerNullReferenceException()
        {
            string[] values = { "one", null, "two" };
            for (int ctr = 0; ctr <= values.GetUpperBound(0); ctr++)
            {
                var val = values[ctr].Trim();
                var separator = ctr == values.GetUpperBound(0) ? "" : ", ";
                Debug.WriteLine("{0}{1}", val, separator);
            }
            Debug.WriteLine("");
        }

        void AggregateException(object sender, EventArgs e)
        {
            HandleOrThrow(() => throw PrepareException());
        }

        static Exception PrepareException()
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

        static Exception SendHttp()
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

        static Exception ValidateLength()
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

        public async void AsyncException(object sender, EventArgs e)
        {
            try
            {
                await FakeService.DoStuffInBackground();
            }
            catch (Exception ex) when (HandleExceptionsSwitchCell.On)
            {
                TrackException(ex);
            }
        }

        private void TrackException(Exception e)
        {
            var properties = new Dictionary<string, string>();
            foreach (Property property in Properties)
            {
                properties.Add(property.Name, property.Value);
            }
            if (properties.Count == 0)
            {
                properties = null;
            }
            Properties.Clear();
            RefreshPropCount();
            Crashes.TrackError(e, properties, App.GetErrorAttachments().ToArray());
        }

        void ClearCrashUserConfirmation(object sender, EventArgs e)
        {
            DependencyService.Get<IClearCrashClick>().ClearCrashButton();
        }

        void MemoryWarningTrigger(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                var blockSize = 128 * 1024 * 1024;
                var byteArrays = new List<IEnumerable<byte>>();
                while (true)
                {
                    byteArrays.Add(Enumerable.Repeat((byte)blockSize, 10000000));
                }
            });
        }
    }
}
