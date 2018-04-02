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


            if (Config.SaveStyle == SaveStyles.Json)
                JsonRadio.IsChecked = true;
            else
                NamespaceRadio.IsChecked = true;

            PageSizeText.Text = importOptions.PageSize.ToString();

        }

        private void SaveOptions(object sender, RoutedEventArgs e)
        {
            var newOptions = new AppOptions();
            if (JsonRadio.IsChecked.GetValueOrDefault())
            {
                newOptions.SaveStyle = SaveStyles.Json;
            }
            else
                newOptions.SaveStyle = SaveStyles.Namespaced;

            newOptions.PageSize = Convert.ToInt32(PageSizeText.Text);
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
