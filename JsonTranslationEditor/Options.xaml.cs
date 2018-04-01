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

namespace JsonTranslationEditor
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public AppOptions Config { get; private set; }

        public Options(AppOptions importOptions)
        {
            Config = importOptions;

            InitializeComponent();


            if (Config.SaveStyle == JsonHelper.SaveStyles.Json)
                JsonRadio.IsChecked = true;
            else
                NamespaceRadio.IsChecked = true;



        }

        private void SaveOptions(object sender, RoutedEventArgs e)
        {
            var newOptions = new AppOptions();
            if (JsonRadio.IsChecked.GetValueOrDefault())
            {
                newOptions.SaveStyle = JsonHelper.SaveStyles.Json;
            }
            else
                newOptions.SaveStyle = JsonHelper.SaveStyles.Namespaced;

            newOptions.DefaultPath = Config.DefaultPath;
            Config = newOptions;
            newOptions.ToDisk();
            DialogResult = true;
        }

        private void CloseOptions(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
