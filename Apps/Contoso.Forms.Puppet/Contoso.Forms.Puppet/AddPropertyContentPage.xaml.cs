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

        void AddProperty(object sender, EventArgs e)
        {
            Property addedProperty = new Property(NameCell.Text, ValueCell.Text);
            PropertyAdded.Invoke(addedProperty);
            Navigation.PopModalAsync();
        }

        void Cancel(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

    }
}
