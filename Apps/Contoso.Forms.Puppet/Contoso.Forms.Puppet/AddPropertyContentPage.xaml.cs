using System;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AddPropertyContentPage : ContentPage
    {
        public event Action<Property> PropertyAdded;
        public AddPropertyContentPage()
        {
            InitializeComponent();
        }

        async void AddProperty(object sender, EventArgs e)
        {
            Property addedProperty = new Property(NameCell?.Text, ValueCell?.Text);
            PropertyAdded?.Invoke(addedProperty);
            await Navigation.PopModalAsync();
        }

        async void Cancel(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

    }
}
