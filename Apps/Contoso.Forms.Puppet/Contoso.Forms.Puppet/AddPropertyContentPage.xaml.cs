using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public partial class AddPropertyContentPage : ContentPage
    {
        public event Action<Property> PropertyAdded;

        public AddPropertyContentPage()
        {
            InitializeComponent();
        }

        void AddProperty(object sender, System.EventArgs e)
        {
            Property addedProperty = new Property(NameCell.Text, ValueCell.Text);
            PropertyAdded.Invoke(addedProperty);
            Navigation.PopModalAsync();
        }

        void Cancel(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

    }
}
