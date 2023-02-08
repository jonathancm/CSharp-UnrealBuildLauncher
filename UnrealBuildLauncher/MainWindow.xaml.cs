// Copyright(C) 2023 Jonathan Caron-Mailhot - All Rights Reserved

using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace UnrealBuildLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* Member Variables */
        private Dictionary<string, BuildConfigCategory> CategoryWidgets = new Dictionary<string, BuildConfigCategory>();

        public MainWindow()
        {
            InitializeComponent();

            RefreshWindow();
        }

        public void RefreshWindow()
        {
            // Reset Window
            ClearErrorText();
            ClearConfigWidgets();

            // Fetch build configs
            List<BuildConfigData> BuildConfigs = LoadBuildConfigs();

            // Populate Categories
            foreach (var BuildConfig in BuildConfigs)
            {
                if (string.IsNullOrEmpty(BuildConfig.BuildCategory))
                    continue;

                if (CategoryWidgets.ContainsKey(BuildConfig.BuildCategory))
                    continue;

                CategoryWidgets[BuildConfig.BuildCategory] = new BuildConfigCategory();
                CategoryWidgets[BuildConfig.BuildCategory].SetCategory(BuildConfig.BuildCategory);
                CategoriesStackPanel.Children.Add(CategoryWidgets[BuildConfig.BuildCategory]);
            }

            // Populate Entries
            foreach (var BuildConfig in BuildConfigs)
            {
                if (string.IsNullOrEmpty(BuildConfig.BuildCategory))
                    continue;

                if (!CategoryWidgets.ContainsKey(BuildConfig.BuildCategory))
                    continue;

                CategoryWidgets[BuildConfig.BuildCategory].AddEntryWidget(BuildConfig);
            }
        }

        public List<BuildConfigData> LoadBuildConfigs()
        {
            var OutData = new List<BuildConfigData>();

            string ConfigFilePath = GetConfigFilePath();
            if (string.IsNullOrEmpty(ConfigFilePath))
            {
                SetErrorText("No config file specified.");
                return OutData;
            }

            if (!File.Exists(ConfigFilePath))
            {
                SetErrorText("Config file not found at \"" + ConfigFilePath + "\".");
                return OutData;
            }

            string TextContent = File.ReadAllText(ConfigFilePath);
            if (string.IsNullOrEmpty(TextContent))
            {
                SetErrorText("Config file seems to be empty!");
                return OutData;
            }

            BuildConfigsFile? BuildConfigsFile = null;
            try
            {
                JsonSerializerOptions Options = new JsonSerializerOptions();
                Options.AllowTrailingCommas = true;
                BuildConfigsFile = JsonSerializer.Deserialize<BuildConfigsFile>(TextContent, Options);
            }
            catch
            {
                SetErrorText("Failed to extract build configurations! The config file contains invalid JSON!");
                return OutData;
            }
            if (BuildConfigsFile == null)
            {
                SetErrorText("Failed to extract build configurations from config file! Verify file format!");
                return OutData;
            }

            if (BuildConfigsFile.BuildConfigs.Count == 0)
            {
                SetErrorText("Config file does not contain any build configuration!");
                return OutData;
            }

            foreach (var BuildConfig in BuildConfigsFile.BuildConfigs)
                OutData.Add(BuildConfig);

            return OutData;
        }

        public string GetConfigFilePath()
        {
            return Properties.Settings.Default.ConfigFileLocation;
        }

        public void SetErrorText(string ErrorText)
        {
            StatusIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF900021"));
            TextErrorPrompt.Text = ErrorText;
        }

        public void ClearErrorText()
        {
            StatusIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF009051"));
            TextErrorPrompt.Text = "";
        }

        public void ClearConfigWidgets()
        {
            foreach (var CategoryWidget in CategoryWidgets.Values)
            {
                CategoryWidget.ClearConfigWidgets();
            }
            CategoriesStackPanel.Children.Clear();
            CategoryWidgets.Clear();
        }

        public void OnClick_NewFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                Properties.Settings.Default.ConfigFileLocation = saveFileDialog.FileName;
                Properties.Settings.Default.Save();

                Stream stream;
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    // Create Template Data
                    var ConfigFileTemplate = new BuildConfigsFile();
                    ConfigFileTemplate.InitTemplate();

                    // Serialize to JSON
                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.WriteIndented = true;
                    string defaultFileContent = JsonSerializer.Serialize(ConfigFileTemplate, options);

                    // Write to file
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(defaultFileContent);
                    }

                    stream.Close();
                }

                RefreshWindow();
            }
        }

        public void OnClick_OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json) |*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                Properties.Settings.Default.ConfigFileLocation = openFileDialog.FileName;
                Properties.Settings.Default.Save();

                RefreshWindow();
            }
        }

        public void OnClick_SaveFile(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature is not yet implemented :P");

            string ConfigFilePath = GetConfigFilePath();
        }

        public void OnClick_SaveFileAs(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature is not yet implemented :P");
        }

        public void OnClick_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
