// Copyright(C) 2023 Jonathan Caron-Mailhot - All Rights Reserved

namespace UnrealBuildLauncher
{
    public class BuildConfigData
    {
        public string BuildName { get; set; } = "";
        public string BuildCategory { get; set; } = "";
        public string ExecPath { get; set; } = "";
        public string ExecArgs { get; set; } = "";

        public void Sanitize()
        {
            BuildName.TrimStart();
            BuildName.TrimEnd();

            BuildCategory.TrimStart();
            BuildCategory.TrimEnd();

            ExecPath.TrimStart();
            ExecPath.TrimEnd();

            ExecArgs.TrimStart();
            ExecArgs.TrimEnd();
        }
    }
}
