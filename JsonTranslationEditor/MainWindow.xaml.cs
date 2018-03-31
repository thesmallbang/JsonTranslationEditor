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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JsonTranslationEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<LanguageSetting> allSettings;
        private TreeViewItem selectedNode;
        private SummaryInfo summaryInfo = new SummaryInfo();

        private string startupPath { get; set; }

        public MainWindow(string startupPath)
        {
            InitializeComponent();
            this.startupPath = startupPath;

            RoutedCommand saveCommand = new RoutedCommand();
            saveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(saveCommand, Save));

            RoutedCommand refreshCommand = new RoutedCommand();
            refreshCommand.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(refreshCommand, Refresh));


            RoutedCommand deleteCommand = new RoutedCommand();
            deleteCommand.InputGestures.Add(new KeyGesture(Key.Delete, ModifierKeys.None));
            CommandBindings.Add(new CommandBinding(deleteCommand, DeleteItem));

            RoutedCommand renameCommand = new RoutedCommand();
            renameCommand.InputGestures.Add(new KeyGesture(Key.F2, ModifierKeys.None));
            CommandBindings.Add(new CommandBinding(renameCommand, RenameItem));

            RoutedCommand newCommand = new RoutedCommand();
            newCommand.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(newCommand, NewItem));

            RoutedCommand newLanguageCommand = new RoutedCommand();
            newLanguageCommand.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(newLanguageCommand, NewLanguage));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            if (!string.IsNullOrWhiteSpace(startupPath))
            {
                LoadFolder(startupPath);
            }
        }

        private void LoadFolder(string path)
        {
            itemsControl.ItemsSource = null;
            allSettings = new JsonHelper().Load(path);
            summaryInfo.Update(allSettings);
            summaryControl.ItemsSource = null;
            summaryControl.ItemsSource = summaryInfo.Details;
            AddMissingLanguages();
            SetupTree();

        }
        private void SetupTree()
        {
            summaryInfo.Update(allSettings);
            this.TreeNamespace.Items.Clear();
            var root = CreateTreeNodes(allSettings);
            var nodes = new List<TreeViewItem>();
            foreach (TreeViewItem node in root.Items)
            {
                nodes.Add(node);
            }
            root.Items.Clear();

            foreach (var node in nodes)
            {
                this.TreeNamespace.Items.Add(node);
            }
            this.TreeNamespace.SelectedItemChanged += TreeNamespace_SelectedItemChanged;

            itemMenu.IsEnabled = false;

        }

        private void TreeNamespace_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectedNode = (TreeViewItem)e.NewValue;
            if (selectedNode == null)
            {
                //disable item menu
                itemMenu.IsEnabled = false;
                return;
            }

            selectedNode.IsExpanded = true;

            itemMenu.IsEnabled = true;
            //enable item menu

            var clickedNamespace = BuildNamespaceFromNode(selectedNode);
            if (string.IsNullOrWhiteSpace(clickedNamespace) && selectedNode != null)
            {
                itemsControl.ItemsSource = null;
                //change to send all
                return;
            }
            if (string.IsNullOrWhiteSpace(clickedNamespace))
            {
                itemsControl.ItemsSource = null;
                return;
            }

            var matchedSettings = allSettings.Where(o => o.Namespace.StartsWith(clickedNamespace));

            var namespaces = matchedSettings.Select(o => o.Namespace).Distinct().ToList();
            var languageGroups = new List<LanguageGroup>();
            foreach (string ns in namespaces)
            {
                var languageGroup = new LanguageGroup(ns,allSettings.Select(o=>o.Language).ToArray());
                languageGroup.LoadSettings(matchedSettings.Where(o => o.Namespace == ns).ToList());
                languageGroups.Add(languageGroup);
            }

            itemsControl.ItemsSource = languageGroups;

        }

        private TreeViewItem CreateTreeNodes(List<LanguageSetting> all)
        {
            var namespaces = all.Select(o => o.Namespace.Split('.')[0]).Distinct().OrderBy(o => o).ToList();
            var root = new TreeViewItem() { Header = "root", Tag = "Assets/Images/ns.png" };


            foreach (var ns in namespaces)
            {
                ProcessNs(root, ns, all);
            }

            return root;
        }
        private void ProcessNs(TreeViewItem node, string ns, List<LanguageSetting> allSettings)
        {
            var thisNode = new TreeViewItem() { Header = (ns.Split('.').Last()), Tag = node.Tag };

            if (node == null)
                node = thisNode;
            else
                node.Items.Add(thisNode);

            var namespaces = allSettings.Where(o => o.Namespace.StartsWith(ns + ".")).Select(o => o.Namespace.Substring(ns.Length + 1).Split('.')[0]).Distinct().OrderBy(o => o).ToList();

            if (!namespaces.Any())
            {
                thisNode.Tag = "Assets/Images/translation.png";
            }

            foreach (var nextNs in namespaces)
            {
                ProcessNs(thisNode, ns + "." + nextNs, allSettings);
            }

        }

        private void AddMissingLanguages()
        {
            var namespaces = allSettings.Select(o => o.Namespace).Distinct().ToList();
            var allLanguages = allSettings.Select(o => o.Language).Distinct().ToList();

            foreach (var language in allLanguages)
            {
                var languageNamespaces = allSettings.Where(o => o.Language == language).Select(o => o.Namespace).Distinct().ToList();
                allSettings.AddRange(namespaces.Except(languageNamespaces).Select(o=> new LanguageSetting() {Namespace = o,Value = string.Empty,Language = language }));
            }

        }
        private string BuildNamespaceFromNode(TreeViewItem node, string built = "")
        {
            if (node == null) return built;

            if (node.Parent == null && string.IsNullOrEmpty(built))
                return node.Header.ToString();


            if (node.Parent is TreeViewItem)
            {
                return BuildNamespaceFromNode((TreeViewItem)node.Parent, built = string.IsNullOrEmpty(built) ? node.Header.ToString() : node.Header.ToString() + "." + built);
            }
            else
            {
                return node.Header + "." + built;
            }

        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var txtBox = (TextBox)sender;
            LanguageSetting setting = (LanguageSetting)txtBox.Tag;
            setting.Value = txtBox.Text;

            summaryInfo.Update(allSettings);
            summaryControl.ItemsSource = null;
            summaryControl.ItemsSource = summaryInfo.Details;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var seperatedByLanguage = allSettings.GroupBy(o => o.Language).Select(o => new { Language = o.Key, Settings = o.Select(p => p) });
            var dictionary = new Dictionary<string, IEnumerable<LanguageSetting>>();

            foreach (var matches in seperatedByLanguage)
            {
                dictionary.Add(matches.Language, matches.Settings);
            }

            new JsonHelper().SaveSettings(0, startupPath, dictionary);
            Console.WriteLine("Saved");
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            LoadFolder(startupPath);

            Console.WriteLine("Refreshed");

        }

        private void NewItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = (TreeViewItem)TreeNamespace.SelectedItem;


            var ns = BuildNamespaceFromNode(node);

            var dialog = new Prompt("New Translation","Enter the translation name below.", ns);
            if (dialog.ShowDialog() != true)
                return;

            if (string.IsNullOrWhiteSpace(dialog.ResponseText))
                return;

            if (allSettings.Any(setting => setting.Namespace.Contains(dialog.ResponseText)))
            {
                MessageBox.Show("Duplicate name");
                return;
            }

            var languages = allSettings.Select(o => o.Language).Distinct().ToList();
            var counter = 0;
            foreach (var language in languages)
            {
                var val = string.Empty;
                counter++;
                if (counter == 1)
                    val = "missingtranslation_"+dialog.ResponseText;

                allSettings.Add(new LanguageSetting() { Namespace = dialog.ResponseText, Value = val, Language = language });
            }

            itemsControl.ItemsSource = null;
            SetupTree();
            
        }


        private void NewLanguage(object sender, RoutedEventArgs e)
        {
            
            var dialog = new Prompt("New Language", "Enter the translation language name below.");
            if (dialog.ShowDialog() != true)
                return;

            if (string.IsNullOrWhiteSpace(dialog.ResponseText))
                return;

            if (allSettings.Any(setting => setting.Language == dialog.ResponseText))
            {
                MessageBox.Show("Duplicate language");
                return;
            }
                        
            var toCopy = allSettings.First();
            var newSetting = new LanguageSetting() { Namespace = toCopy.Namespace, Value = "fillmein", Language = dialog.ResponseText };

            allSettings.Add(newSetting);

            itemsControl.ItemsSource = null;
            AddMissingLanguages();
            SetupTree();
        }
        private void RenameItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = (TreeViewItem)TreeNamespace.SelectedItem;

            if (node == null)
                return;

            var ns = BuildNamespaceFromNode(node);
            var originalName = node.Header.ToString();

            var dialog = new Prompt("Rename: " + originalName, "Enter the new name below.", originalName);
            if (dialog.ShowDialog() != true)
                return;

            if (string.IsNullOrWhiteSpace(dialog.ResponseText))
                return;

            if (dialog.ResponseText.Contains("."))
                return;


            if (!(node.Parent is TreeViewItem))
            {
                dialog.ResponseText += ".";

            }

            var newNs = ns.Substring(0, ns.LastIndexOf(node.Header.ToString())) + dialog.ResponseText.Trim();


            allSettings.ForEach((item) =>
            {
                if (item.Namespace.StartsWith(ns))
                    item.Namespace = item.Namespace.Replace(ns, newNs);
            });

            SetupTree();


        }
        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = (TreeViewItem)TreeNamespace.SelectedItem;

            if (node == null)
                return;

            var ns = BuildNamespaceFromNode(node);
            if (string.IsNullOrWhiteSpace(ns))
                return;

            if (node.Parent is TreeViewItem)
            {
                ((TreeViewItem)node.Parent).Items.Remove(node);
            }
            else
            {
                TreeNamespace.Items.Remove(node);

            }
            TreeNamespace.UpdateLayout();
            allSettings.RemoveAll(o => o.Namespace.StartsWith(ns));

        }

    }
}
