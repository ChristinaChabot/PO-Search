using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using System.Xaml;
using System.Xml;
using System.Windows.Markup;


namespace PO_Search
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            slSize.Value = 2;
            slDarklight.Value = 1;

        }
        ResourceDictionary _ResourceDictionary = new ResourceDictionary();
        private void slSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (slSize.Value == 0)
            {
                lblSize.Content = "Large";
                Application.Current.MainWindow.FontSize = 20;

            }
            else if (slSize.Value == 1)
            {
                lblSize.Content = "Medium";
                Application.Current.MainWindow.FontSize = 16;
                

            }
            else
            {
                lblSize.Content = "Small";
                Application.Current.MainWindow.FontSize = 12;
                

            }

        }
        private void SlDarklight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slDarklight.Value == 0)
            {
                lblDarklight.Content = "Light";

                _ResourceDictionary.Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(_ResourceDictionary);


            }
            else
            {
                lblDarklight.Content = "Dark";
               
                _ResourceDictionary.Source = new Uri("/Themes/DarkTheme.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(_ResourceDictionary);



            }
        }
    }

}
