// Copyright(C) 2023 Jonathan Caron-Mailhot - All Rights Reserved

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
        const string ConfigFileName = "build_configs.json";

        public MainWindow()
        {
            InitializeComponent();

            // Cleanup placeholder controls
            CategoriesStackPanel.Children.Clear();

            // Fetch build configs
            List<BuildConfigData> BuildConfigs = LoadBuildConfigs();

            // Populate Categories
            var CategoryWidgets = new Dictionary<string, BuildConfigCategory>();
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
            if (!File.Exists(ConfigFilePath))
            {
                SetErrorText("Config file not found at \"" + ConfigFilePath + "\"");
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

            if (BuildConfigsFile.BuildConfigs.Length == 0)
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
#if DEBUG
            string BaseFileDirectory = Path.GetFullPath("./../../../");
#else
            string BaseFileDirectory = Path.GetFullPath("./");
#endif
            return BaseFileDirectory + ConfigFileName;
        }

        public void SetErrorText(string ErrorText)
        {
            TextErrorTitle.Foreground = new SolidColorBrush(Colors.Red);
            TextErrorPrompt.Text = ErrorText;
        }

        public void ClearErrorText()
        {
            TextErrorTitle.Foreground = new SolidColorBrush(Colors.White);
            TextErrorPrompt.Text = "OK";
        }
    }
}
