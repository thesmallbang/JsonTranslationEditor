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
    /// Interaction logic for Prompt.xaml
    /// </summary>
    partial class Prompt : Window
    {



        public Prompt(string title, string message, string defaultValue = "")
        {
            InitializeComponent();
            Title = title;
            messageLabel.Text = message;
            InitializeComponent();
            ResponseTextBox.Text = defaultValue;
            ResponseTextBox.Focus();
            ResponseTextBox.SelectAll();

            RoutedCommand saveCommand = new RoutedCommand();
            saveCommand.InputGestures.Add(new KeyGesture(Key.Enter, ModifierKeys.None));
            CommandBindings.Add(new CommandBinding(saveCommand, OKButton_Click));

            RoutedCommand refreshCommand = new RoutedCommand();
            refreshCommand.InputGestures.Add(new KeyGesture(Key.Escape, ModifierKeys.None));
            CommandBindings.Add(new CommandBinding(refreshCommand, CancelDialog));



        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }


        private void CancelDialog(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
