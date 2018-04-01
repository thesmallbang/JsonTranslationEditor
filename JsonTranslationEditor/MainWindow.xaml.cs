using Ookii.Dialogs.Wpf;
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
        private PagingController<LanguageGroup> pagingController = new PagingController<LanguageGroup>(30,new List<LanguageGroup>());

        private string currentPath { get; set; }

        public MainWindow(string startupPath)
        {
            InitializeComponent();
            this.currentPath = startupPath;

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
            newLanguageCommand.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(newLanguageCommand, NewLanguage));

            RoutedCommand openFolderCommand = new RoutedCommand();
            openFolderCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(openFolderCommand, OpenFolder));

            RoutedCommand nextPageCommand = new RoutedCommand();
            nextPageCommand.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Alt));
            CommandBindings.Add(new CommandBinding(nextPageCommand, NextPage));

            RoutedCommand previousPageCommand = new RoutedCommand();
            previousPageCommand.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Alt));
            CommandBindings.Add(new CommandBinding(previousPageCommand, PreviousPage));

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.TreeNamespace.SelectedItemChanged += TreeNamespace_SelectedItemChanged;
            SearchFilterTextbox.TextChanged += SearchFilterTextbox_TextChanged;
            if (!string.IsNullOrWhiteSpace(currentPath))
            {
                LoadFolder(currentPath);
            }
        }



        private void LoadFolder(string path)
        {
            allSettings = new JsonHelper().Load(path).ToList();
            AddMissingTranslations();
            RefreshTree();
            UpdateSummaryInfo();
            

        }

        private void RefreshTree(string selectNamespace = "")
        {
            TreeNamespace.Items.Clear();
            var nodes = allSettings.ForParse().ToTreeItems();
            foreach (var node in nodes)
            {
                this.TreeNamespace.Items.Add(node);
            }
            itemMenu.IsEnabled = false;

            if (!string.IsNullOrEmpty(selectNamespace))
            {
               TreeNamespace.Items.SelectByNamespace(selectNamespace);
            }
        }

        private void UpdateSummaryInfo()
        {
            summaryInfo.Update(allSettings);
            summaryControl.ItemsSource = null;
            summaryControl.ItemsSource = summaryInfo.Details;
        }

        private void TreeNamespace_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectedNode = (TreeViewItem)e.NewValue;
            if (selectedNode == null)
            {
                itemMenu.IsEnabled = false;
                return;
            }
            selectedNode.IsExpanded = true;
            itemMenu.IsEnabled = true;

            
            var clickedNamespace = selectedNode.ToNamespaceString();
            if (selectedNode.HasItems)
                clickedNamespace += ".";
            
            
            if (string.IsNullOrWhiteSpace(clickedNamespace))
            {
                return;
            }

            SearchFilterTextbox.Text = clickedNamespace.Replace("..", ".");
        }

        private void SearchFilterTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {

            var matchedSettings = allSettings.ForParse().ToList();
            if (SearchFilterTextbox.Text.EndsWith("."))
            {
                matchedSettings = matchedSettings.Where(o => o.Namespace.StartsWith(SearchFilterTextbox.Text)).ToList();
            } else
            {
                matchedSettings = matchedSettings.Where(o => o.Namespace == SearchFilterTextbox.Text).ToList();
            }

            var namespaces = matchedSettings.ToNamespaces().ToList();
            var languages = allSettings.ToLanguages().ToList();

            var languageGroups = new List<LanguageGroup>();
            foreach (string ns in namespaces)
            {
                var languageGroup = new LanguageGroup(ns, languages);
                languageGroup.LoadSettings(matchedSettings.Where(o => o.Namespace == ns));
                languageGroups.Add(languageGroup);
            }

            pagingController.SwapData(languageGroups);
            languageGroupContainer.ItemsSource = pagingController.PageData;
            pagingMessage.Text = pagingController.PageMessage;
            ContentScroller.ScrollToTop();


        }

        private void AddMissingTranslations()
        {
            var namespaces = allSettings.ToNamespaces().ToList();
            var allLanguages = allSettings.ToLanguages().ToList();

            foreach (var language in allLanguages)
            {
                var languageNamespaces = allSettings.OnlyLanguage(language).ToNamespaces();
                allSettings.AddRange(namespaces.Except(languageNamespaces).Select(o => new LanguageSetting() { Namespace = o, Value = string.Empty, Language = language }));
            }

        }




        private void LanguageValue_KeyUp(object sender, KeyEventArgs e)
        {
            var txtBox = (TextBox)sender;
            LanguageSetting setting = (LanguageSetting)txtBox.Tag;
            setting.Value = txtBox.Text;
            UpdateSummaryInfo();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            new JsonHelper().SaveSettings(0, currentPath, allSettings.ToLanguageDictionary());
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            LoadFolder(currentPath);
        }
        private void NewItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = (TreeViewItem)TreeNamespace.SelectedItem;


            var ns = node.ToNamespaceString();

            var dialog = new Prompt("New Translation", "Enter the translation name below.", ns);
            if (dialog.ShowDialog() != true)
                return;

            if (string.IsNullOrWhiteSpace(dialog.ResponseText))
                return;

            if (allSettings.NoEmpty().Any(setting => setting.Namespace.Contains(dialog.ResponseText)))
            {
                MessageBox.Show("Duplicate name");
                return;
            }

            var languages = allSettings.ToLanguages().ToList();
            foreach (var language in languages)
            {
                var val = string.Empty;
                allSettings.Add(new LanguageSetting() { Namespace = dialog.ResponseText, Value = val, Language = language });
            }
            RefreshTree(dialog.ResponseText);
            UpdateSummaryInfo();
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

            var newSetting = new LanguageSetting() { Namespace = "", Value = "", Language = dialog.ResponseText };

            allSettings.Add(newSetting);
            AddMissingTranslations();
            UpdateSummaryInfo();
            RefreshTree();
            languageGroupContainer.ItemsSource = null;
        }
        private void RenameItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = (TreeViewItem)TreeNamespace.SelectedItem;

            if (node == null)
                return;

            var ns = node.ToNamespaceString();
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


            allSettings.ForParse().ToList().ForEach((item) =>
            {
                if (item.Namespace.StartsWith(ns))
                    item.Namespace = item.Namespace.Replace(ns, newNs);
            });

            RefreshTree(newNs);
        }
        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = (TreeViewItem)TreeNamespace.SelectedItem;

            if (node == null)
                return;

            var ns = node.ToNamespaceString();

            if (string.IsNullOrWhiteSpace(ns))
                return;

            if (!(node.Parent is TreeViewItem))
            {
                TreeNamespace.Items.Remove(node);
            } else
            {
                var parent = ((TreeViewItem)node.Parent);
                parent.Items.Remove(node);
                parent.IsSelected = true;
            }

            allSettings.NoEmpty().ToList().RemoveAll(o => o.Namespace.StartsWith(ns));
        }
        private void OpenFolder(object sender, RoutedEventArgs e)
        {

            var dialog = new VistaFolderBrowserDialog();
            dialog.SelectedPath = currentPath;
            var selected = dialog.ShowDialog(this);
            if (selected.GetValueOrDefault())
            {
                currentPath = dialog.SelectedPath;
                LoadFolder(currentPath);
            }
        }

        private void NextPage(object sender, RoutedEventArgs e)
        {
            if (pagingController == null || !pagingController.HasNextPage)
                return;
            pagingController.NextPage();
            PagedUpdates();
        }
        private void PreviousPage(object sender, RoutedEventArgs e)
        {
            if (pagingController == null || !pagingController.HasPreviousPage)
                return;
            pagingController.PreviousPage();
            PagedUpdates();
        }

        private void PagedUpdates()
        {
            languageGroupContainer.ItemsSource = pagingController.PageData;
            pagingMessage.Text = pagingController.PageMessage;
            ContentScroller.ScrollToTop();
        }

    }
}
