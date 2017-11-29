using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Contoso.Forms.Puppet.Views
{
    public partial class TextAttachmentView : ContentPage
    {
        private readonly TaskCompletionSource<string> _taskCompletionSource = new TaskCompletionSource<string>();
        private INavigation _navigation;

        public TextAttachmentView()
        {
            InitializeComponent();
            On<iOS>().SetUseSafeArea(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Text.Focus();
        }

        public static Task<string> Show(INavigation navegation)
        {
            var view = new TextAttachmentView();
            view._navigation = navegation;
            navegation.PushModalAsync(view);
            return view._taskCompletionSource.Task;
        }

        async void OK(object sender, EventArgs args)
        {
            var result = Text.Text;
            await _navigation.PopModalAsync();
            _taskCompletionSource.SetResult(result);
        }

        async void Cancel(object sender, EventArgs args)
        {
            await _navigation.PopModalAsync();
            _taskCompletionSource.SetResult(null);
        }
    }
}
