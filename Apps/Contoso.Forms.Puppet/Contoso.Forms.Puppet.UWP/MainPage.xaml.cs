namespace Contoso.Forms.Puppet.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadApplication(new Puppet.App());
        }
    }
}
