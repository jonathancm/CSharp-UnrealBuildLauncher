using Microsoft.Win32;

using System.Windows;

namespace UnrealBuildLauncher
{
    /// <summary>
    /// Interaction logic for ConfigEditWindow.xaml
    /// </summary>
    public partial class ConfigEditWindow : Window
    {
        public BuildConfigData ConfigData { get; private set; } = new BuildConfigData();

        public ConfigEditWindow(BuildConfigData Data)
        {
            InitializeComponent();

            ConfigData = Data;
            TextBoxConfigName.Text = ConfigData.BuildName;
            TextBoxConfigCategory.Text = ConfigData.BuildCategory;
            TextBoxExecPath.Text = ConfigData.ExecPath;
            TextBoxExecArgs.Text = ConfigData.ExecArgs;
        }

        private void OnClick_Apply(object sender, RoutedEventArgs e)
        {
            ConfigData.BuildName = TextBoxConfigName.Text;
            ConfigData.BuildCategory = TextBoxConfigCategory.Text;
            ConfigData.ExecPath = TextBoxExecPath.Text;
            ConfigData.ExecArgs = TextBoxExecArgs.Text;
            ConfigData.Sanitize();

            DialogResult = true;
            Close();
        }

        private void OnClick_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnClick_OpenFileDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "EXE files (*.exe) |*.exe|BAT files (*.bat)|*.bat|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                TextBoxExecPath.Text = openFileDialog.FileName;
            }
        }
    }
}
