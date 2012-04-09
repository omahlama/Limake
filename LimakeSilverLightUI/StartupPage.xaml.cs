using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace LimakeSilverLightUI
{
    public partial class StartupPage : Page
    {
        public StartupPage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String green="",red="",blue="",yellow="";
            if (GH.IsChecked == true) green = "Human";
            else if (GR.IsChecked == true) green = "Random";
            else if (GS.IsChecked == true) green = "Basic";

            if (RH.IsChecked == true) red = "Human";
            else if (RR.IsChecked == true) red = "Random";
            else if (RS.IsChecked == true) red = "Basic";

            if (BH.IsChecked == true) blue = "Human";
            else if (BR.IsChecked == true) blue = "Random";
            else if (BS.IsChecked == true) blue = "Basic";

            if (YH.IsChecked == true) yellow = "Human";
            else if (YR.IsChecked == true) yellow = "Random";
            else if (YS.IsChecked == true) yellow = "Basic";


            NavigationService.Navigate(new Uri("/MainPage.xaml?Green="+green+"&Red="+red+"&Blue="+blue+"&Yellow="+yellow, UriKind.Relative));
        }

    }
}
