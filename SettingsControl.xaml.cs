using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Orientation = System.Windows.Controls.Orientation;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace Flow.Launcher.Plugin.FileFolderOpener
{
    public partial class SettingsControl : UserControl
    {
        private readonly Settings _settings;
        public ObservableCollection<FileFolder> Folders { get; set; }

        public SettingsControl(PluginInitContext context, SettingsViewModel viewModel)
        {
            InitializeComponent();
            _settings = viewModel.Settings;
            DataContext = _settings;
            Folders = viewModel.Settings.Folders;
        }

        private void OnSelectIconClick(object sender, RoutedEventArgs e)
        {
            var folder = dataGrid.SelectedItem as FileFolder;
       
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.ico|All Files|*.*",
                Title = "Select an Icon"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (folder == null)
                {
                    folder = new FileFolder();
                    Folders.Add(folder);
                    dataGrid.SelectedItem = folder;
                }

                folder.IconPath = openFileDialog.FileName;
            }
        }

        private void OnDeleteIconClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is FileFolder folder)
            {
                dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                folder.IconPath = string.Empty;
                dataGrid.Items.Refresh();
            }
        }

        private void OnSelectFolderClick(object sender, RoutedEventArgs e)
        {
            var folder = dataGrid.SelectedItem as FileFolder;

            var dialog = new Window
            {
                Title = "Selection Type",
                Width = 320,
                Height = 170,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = Window.GetWindow(this)
            };

            var panel = new StackPanel { Margin = new Thickness(20), Orientation = Orientation.Vertical };

            panel.Children.Add(new TextBlock
            {
                Text = "What do you want to select?",
                Margin = new Thickness(0, 0, 0, 20),
                TextAlignment = TextAlignment.Center,
                FontSize = 14
            });

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment =  System.Windows.HorizontalAlignment.Center
            };

            var folderButton = new Button { Content = "📁 Folder", Width = 80, Height = 40, Margin = new Thickness(5), Tag = "folder" };
            var fileButton = new Button { Content = "📄 File", Width = 80, Height = 40, Margin = new Thickness(5), Tag = "file" };
            var cancelButton = new Button { Content = "Cancel", Width = 80, Height = 40, Margin = new Thickness(5), Tag = "cancel" };

            folderButton.Click += (s, args) => { dialog.Tag = "folder"; dialog.Close(); };
            fileButton.Click += (s, args) => { dialog.Tag = "file"; dialog.Close(); };
            cancelButton.Click += (s, args) => { dialog.Tag = "cancel"; dialog.Close(); };

            buttonPanel.Children.Add(folderButton);
            buttonPanel.Children.Add(fileButton);
            buttonPanel.Children.Add(cancelButton);

            panel.Children.Add(buttonPanel);
            dialog.Content = panel;

            dialog.ShowDialog();

            var result = dialog.Tag?.ToString();
            if (result == null || result == "cancel")
                return;

            string selectedPath = null;

            if (result == "folder")
            {
                using var folderDialog = new FolderBrowserDialog { Description = "Select a folder" };
                if (folderDialog.ShowDialog() == DialogResult.OK)
                    selectedPath = folderDialog.SelectedPath;
            }
            else if (result == "file")
            {
                var openFileDialog = new OpenFileDialog { Filter = "All Files|*.*", Title = "Select a File" };
                if (openFileDialog.ShowDialog() == true)
                    selectedPath = openFileDialog.FileName;
            }

            if (!string.IsNullOrEmpty(selectedPath) && sender is Button button)
            {
                if (folder == null)
                {
                    folder = new FileFolder();
                    Folders.Add(folder);
                    dataGrid.SelectedItem = folder;
                }
                folder.Path = selectedPath;
            }
        }

        private void OnEditTextBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Focus();
                textBox.SelectAll();
            }
        }

        private void OnPathTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                var currentRowIndex = dataGrid.SelectedIndex;
                if (currentRowIndex < dataGrid.Items.Count - 1)
                {
                    dataGrid.SelectedIndex = currentRowIndex + 1;
                    dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[currentRowIndex + 1], dataGrid.Columns[2]);
                    dataGrid.BeginEdit();
                }

                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                dataGrid.CancelEdit();
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)
            {
                dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            }
        }
    }
}
