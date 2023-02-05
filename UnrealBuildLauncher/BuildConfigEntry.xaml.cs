// Copyright(C) 2023 Jonathan Caron-Mailhot - All Rights Reserved

using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace UnrealBuildLauncher
{
    /// <summary>
    /// Interaction logic for BuildConfigEntry.xaml
    /// </summary>
    public partial class BuildConfigEntry : UserControl
    {
        public BuildConfigData ConfigData { get; private set; } = new BuildConfigData();

        public BuildConfigEntry()
        {
            InitializeComponent();
        }

        public void SetData(BuildConfigData Data)
        {
            ConfigData = Data;
            TextConfigName.Text = ConfigData.BuildName;
        }

        private void OnLaunchButtonClicked(object sender, RoutedEventArgs e)
        {
            if (CanLaunch())
            {
                LaunchBuild();
            }
        }

        private bool CanLaunch()
        {
            return !string.IsNullOrEmpty(ConfigData.ExecPath) && !string.IsNullOrEmpty(ConfigData.ExecArgs);
        }

        private void LaunchBuild()
        {
            var BuildProcess = new Process
            {
                StartInfo =
                {
                    FileName = ConfigData.ExecPath,
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    Arguments = ConfigData.ExecArgs
                }
            };
            BuildProcess.Start(); // Starts detached
        }
    }
}
