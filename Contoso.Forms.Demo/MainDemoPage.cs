using Xamarin.Forms;
using System;
using Contoso.Forms.Demo;

namespace Contoso.Forms.Demo
{
    public partial class MainDemoPage : TabbedPage
    {
        public MainDemoPage ()
        {
            InitializeComponent();

        }

        void LogLevelCellTapped(object sender, System.EventArgs e)
        {
            ((NavigationPage)App.Current.MainPage).PushAsync(new LogLevelPage());
        }
    }
}
