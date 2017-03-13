namespace Contoso.Forms.Puppet.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Puppet.App());
        }
    }
}
