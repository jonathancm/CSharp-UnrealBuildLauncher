// Copyright(C) 2023 Jonathan Caron-Mailhot - All Rights Reserved

using System.Collections.Generic;
using System.Windows.Controls;

namespace UnrealBuildLauncher
{
    /// <summary>
    /// Interaction logic for BuildConfigCategory.xaml
    /// </summary>
    public partial class BuildConfigCategory : UserControl
    {
        /* Member Variables */
        public string BuildCategory { get; private set; } = "";

        public BuildConfigCategory()
        {
            InitializeComponent();

            /* Remove placeholder widgets */
            ClearConfigWidgets();
        }

        public void SetCategory(string Category)
        {
            BuildCategory = Category;
            TextBlockCategoryName.Text = Category;
        }

        public BuildConfigEntry AddEntryWidget(BuildConfigData Data)
        {
            var EntryWidget = new BuildConfigEntry();
            EntryWidget.SetData(Data);
            BuildEntriesStackPanel.Children.Add(EntryWidget);
            return EntryWidget;
        }

        public void ClearConfigWidgets()
        {
            BuildEntriesStackPanel.Children.Clear();
        }

        public List<BuildConfigData> GetEntriesData()
        {
            List<BuildConfigData> buildConfigEntries = new List<BuildConfigData>();
            foreach (BuildConfigEntry Entry in BuildEntriesStackPanel.Children)
            {
                buildConfigEntries.Add(Entry.ConfigData);
            }

            return buildConfigEntries;
        }
    }
}
