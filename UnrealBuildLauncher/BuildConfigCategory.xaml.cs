// Copyright(C) 2023 Jonathan Caron-Mailhot - All Rights Reserved

using System.Windows.Controls;

namespace UnrealBuildLauncher
{
    /// <summary>
    /// Interaction logic for BuildConfigCategory.xaml
    /// </summary>
    public partial class BuildConfigCategory : UserControl
    {
        public BuildConfigCategory()
        {
            InitializeComponent();

            // Cleanup placeholder controls
            BuildEntriesStackPanel.Children.Clear();
        }

        public void SetCategory(string Category)
        {
            BuildCategory = Category;
            TextBlockCategoryName.Text = Category;
        }

        public void AddEntryWidget(BuildConfigData Data)
        {
            var EntryWidget = new BuildConfigEntry();
            EntryWidget.SetData(Data);
            BuildEntriesStackPanel.Children.Add(EntryWidget);
        }

        public string BuildCategory { get; private set; } = "";
    }
}
