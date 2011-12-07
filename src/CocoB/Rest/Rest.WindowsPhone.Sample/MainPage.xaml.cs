using System.Collections.Generic;
using System.Windows;
using CocoB.Rest.WindowsPhone.Core.Parsers;
using Microsoft.Phone.Controls;

namespace CocoB.Rest.WindowsPhone.Sample
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var json = "{\"name\":123,\"name2\":-456e8}";
            var obj = JSON.JsonDecode(json);
            var dict = new Dictionary<string, object>();
            dict["hello"] = "asshole";
            var seri = JSON.JsonEncode(dict);
            int i = 0;
        }
    }
}