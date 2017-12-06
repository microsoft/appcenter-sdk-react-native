using System.Collections.Generic;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public partial class PropertiesContentPage : ContentPage
    {
        public PropertiesContentPage(List<Property> EventProperties)
        {
            InitializeComponent();
            PropertyList.ItemsSource = EventProperties;
        }
    }
}
