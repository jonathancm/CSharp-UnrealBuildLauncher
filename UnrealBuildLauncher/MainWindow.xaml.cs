// Copyright(C) 2023 Jonathan Caron-Mailhot - All Rights Reserved

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace UnrealBuildLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* Member Variables */
        private Dictionary<string, BuildConfigCategory> CategoryWidgets = new Dictionary<string, BuildConfigCategory>();
        private BitmapImage? ImageCheckMark = null;
        private BitmapImage? ImageWarning = null;

        public MainWindow()
        {
            InitializeLog();

            InitializeComponent();
            InitializeImages();

            RefreshWindow();
        }

        private void InitializeLog()
        {
            string appName = System.AppDomain.CurrentDomain.FriendlyName;
            Trace.Listeners.Add(new TextWriterTraceListener(appName + ".log"));
            Trace.IndentSize = 4;
            Trace.AutoFlush = true;
        }

        private void InitializeImages()
        {
            var uriSourceCheckmark = new Uri(@"/UnrealBuildLauncher;component/Assets/check-circle.png", UriKind.Relative);
            ImageCheckMark = new BitmapImage(uriSourceCheckmark);
            var uriSourceWarning = new Uri(@"/UnrealBuildLauncher;component/Assets/exclamation-triangle.png", UriKind.Relative);
            ImageWarning = new BitmapImage(uriSourceWarning);
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
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());

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
            {
                if (string.IsNullOrEmpty(BuildConfig.BuildName))
                {
                    BuildConfig.BuildName = "Unnamed config entry";
                }

                if (string.IsNullOrEmpty(BuildConfig.BuildCategory))
                {
                    BuildConfig.BuildCategory = "Default Category";
                }

                OutData.Add(BuildConfig);
            }

            return OutData;
        }

        public string GetConfigFilePath()
        {
            return Properties.Settings.Default.ConfigFileLocation;
        }

        public void SetErrorText(string ErrorText)
        {
            StatusIcon.Source = ImageWarning;
            TextErrorPrompt.Text = ErrorText;
        }

        public void ClearErrorText()
        {
            StatusIcon.Source = ImageCheckMark;
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
            string ConfigFilePath = GetConfigFilePath();

            if (!File.Exists(ConfigFilePath))
            {
                return;
            }

            // Prepare Template Data
            var ConfigFileTemplate = new BuildConfigsFile();
            foreach (var CategoryWidget in CategoryWidgets)
            {
                ConfigFileTemplate.BuildConfigs.AddRange(CategoryWidget.Value.GetEntriesData());
            }

            // Serialize to JSON
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string fileContent = JsonSerializer.Serialize(ConfigFileTemplate, options);

            // Write to file
            File.WriteAllText(ConfigFilePath, fileContent);
        }

        public void OnClick_SaveFileAs(object sender, RoutedEventArgs e)
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
                    foreach (var CategoryWidget in CategoryWidgets)
                    {
                        ConfigFileTemplate.BuildConfigs.AddRange(CategoryWidget.Value.GetEntriesData());
                    }

                    // Serialize to JSON
                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.WriteIndented = true;
                    string fileContent = JsonSerializer.Serialize(ConfigFileTemplate, options);

                    // Write to file
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(fileContent);
                    }

                    stream.Close();
                }

                RefreshWindow();
            }
        }

        public void OnClick_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
