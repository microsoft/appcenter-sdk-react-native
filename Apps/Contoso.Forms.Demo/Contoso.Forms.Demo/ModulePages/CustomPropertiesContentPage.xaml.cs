using System;
using Microsoft.AppCenter;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class CustomPropertiesContentPage : ContentPage
    {
        public CustomPropertiesContentPage()
        {
            InitializeComponent();
            AddNewProperty();
        }

        public void AddNewProperty()
        {
            PropertiesContainer.Children.Add(new CustomPropertyView());
        }
   
        private void AddProperty_Clicked(object sender, EventArgs e)
        {
            AddNewProperty();
        }

        private void Send_Clicked(object sender, EventArgs e)
        {
            CustomProperties customProperties = new CustomProperties();
            foreach(var customProperty in PropertiesContainer.Children)
            {
                (customProperty as CustomPropertyView).AddCustomProperty(customProperties);
            }
            AppCenter.SetCustomProperties(customProperties);
        }
    }
}
