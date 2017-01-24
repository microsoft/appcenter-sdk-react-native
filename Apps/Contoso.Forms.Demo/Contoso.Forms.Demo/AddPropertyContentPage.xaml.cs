using System;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AddPropertyContentPage
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
