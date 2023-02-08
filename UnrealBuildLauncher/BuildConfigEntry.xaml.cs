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

        private void OnClick_LaunchConfig(object sender, RoutedEventArgs e)
        {
            if (CanLaunch())
            {
                LaunchBuild();
            }
        }

        private bool CanLaunch()
        {
            if (string.IsNullOrEmpty(ConfigData.ExecPath))
                return false;

            if (string.IsNullOrEmpty(ConfigData.ExecArgs))
                return false;

            if (!File.Exists(ConfigData.ExecPath))
                return false;

            return true;
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
