// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class TestPage : ContentPage
    {
        /* CrashResultsPage must persist so that it can subscribe to the crash callbacks */
        public CrashResultsPage ResultsPage = new CrashResultsPage();

        public TestPage()
        {
            InitializeComponent();
        }

        public void GoToTogglePage(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new ToggleStatesPage());
        }

        public void GoToCrashesPage(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new CrashesPage());
        }

        public void GoToAnalyticsPage(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new AnalyticsPage());
        }

        public void GoToCrashResultsPage(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(ResultsPage);
        }

        public void GoToCorePage(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new CorePage());
        }
    }
}
