using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Editor
{
    public class PlayerBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public void OnPostprocessBuild(BuildReport report)
        {
        }

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            BuildPlayer.CopyBundlesToBuild();
        }

    }
}