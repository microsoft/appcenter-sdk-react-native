using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class TestPage : ContentPage
    {
        public TestPage()
        {
            InitializeComponent();
        }

        public void GoToTogglePage(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new ToggleStatesPage());
        }

        public void GoToCrashPage(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new CrashPage()); //TODO going to need to reuse same instance for events to work
        }

        public void GoToAnalyticsPage(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new AnalyticsPage()); //TODO going to need to reuse same instance for events to work
        }
    }
}
