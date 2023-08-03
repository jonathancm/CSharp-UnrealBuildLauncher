// Copyright(C) 2023 Jonathan Caron-Mailhot - All Rights Reserved

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace UnrealBuildLauncher
{
    public delegate void OnBuildConfigEntryModified();

    /// <summary>
    /// Interaction logic for BuildConfigEntry.xaml
    /// </summary>
    public partial class BuildConfigEntry : UserControl
    {
        public BuildConfigData ConfigData { get; private set; } = new BuildConfigData();
        public OnBuildConfigEntryModified? onBuildConfigEntryModified = null;

        public BuildConfigEntry()
        {
            InitializeComponent();
        }

        public void SetData(BuildConfigData Data)
        {
            ConfigData = Data;
            TextConfigName.Text = ConfigData.BuildName;

            string outputError = "";
            if (CanLaunch(out outputError))
            {
                ButtonLaunch.IsEnabled = true;
                ImageConfigWarning.Visibility = Visibility.Collapsed;
                TextConfigDescription.Text = ConfigData.ExecArgs.ToString();
            }
            else
            {
                ButtonLaunch.IsEnabled = false;
                ImageConfigWarning.Visibility = Visibility.Visible;
                TextConfigDescription.Text = outputError;
            }
        }

        private void OnClick_LaunchConfig(object sender, RoutedEventArgs e)
        {
            string outputError = "";
            if (CanLaunch(out outputError))
            {
                LaunchBuild();
            }
        }

        private bool CanLaunch(out string errorOutput)
        {
            if (string.IsNullOrEmpty(ConfigData.ExecPath))
            {
                errorOutput = "Path to executable is empty!";
                return false;
            }

            if (!File.Exists(ConfigData.ExecPath))
            {
                errorOutput = "Could not find executable at specified path!";
                return false;
            }

            errorOutput = "";
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

            try
            {
                BuildProcess.Start(); // Starts detached
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());

                string appName = System.AppDomain.CurrentDomain.FriendlyName;
                MessageBox.Show(exception.ToString(), appName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnClick_EditConfig(object sender, RoutedEventArgs e)
        {
            var configEditWindow = new ConfigEditWindow(ConfigData);

            configEditWindow.Owner = Window.GetWindow(this);
            bool? isConfigApplied = configEditWindow.ShowDialog();
            if (isConfigApplied.HasValue && isConfigApplied.Value == true)
            {
                SetData(configEditWindow.ConfigData);
                if (onBuildConfigEntryModified != null)
                {
                    onBuildConfigEntryModified();
                }
            }
        }
    }
}
