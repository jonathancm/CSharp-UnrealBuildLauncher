// Copyright(C) 2023 Jonathan Caron-Mailhot - All Rights Reserved

using System.Collections.Generic;

namespace UnrealBuildLauncher
{
    public class BuildConfigsFile
    {
        public string Version { get; set; } = "0.2";
        public List<BuildConfigData> BuildConfigs { get; set; } = new List<BuildConfigData>();

        public void InitTemplate()
        {
            BuildConfigs.Add(new BuildConfigData());
        }
    }
}
