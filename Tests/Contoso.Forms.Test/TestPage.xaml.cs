using Xamarin.Forms;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile;

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
            Navigation.PushModalAsync(new CrashesPage()); //TODO going to need to reuse same instance for events to work
        }

        public void GoToAnalyticsPage(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new AnalyticsPage()); //TODO going to need to reuse same instance for events to work
        }

        public void GoToCrashResultsPage(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(ResultsPage);
        }
    }
}
