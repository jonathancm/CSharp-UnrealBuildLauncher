// Copyright(C) 2023 Jonathan Caron-Mailhot - All Rights Reserved

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace UnrealBuildLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
#if DEBUG
        const string ConfileFilePath = "C:\\Users\\jonat\\MyFiles\\CSharp-UnrealBuildLauncher\\UnrealBuildLauncher\\build_configs.json";
#else
        const string ConfileFilePath = "./";
#endif

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

            string TextContent = File.ReadAllText(ConfileFilePath);
            if (string.IsNullOrEmpty(TextContent))
                return OutData;

            var BuildConfigsFile = JsonSerializer.Deserialize<BuildConfigsFile>(TextContent);
            if (BuildConfigsFile == null)
                return OutData;

            if (BuildConfigsFile.BuildConfigs == null)
                return OutData;

            foreach (var BuildConfig in BuildConfigsFile.BuildConfigs)
                OutData.Add(BuildConfig);

            return OutData;
        }
    }
}
