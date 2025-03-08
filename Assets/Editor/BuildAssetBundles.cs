using UnityEditor;
using System.IO;

public class BuildAssetBundles
{
    [MenuItem("Assets/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        string bundleDir = "Assets/StreamingAssets";
        if (!Directory.Exists(bundleDir))
        {
            Directory.CreateDirectory(bundleDir);
        }

        BuildPipeline.BuildAssetBundles(bundleDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}