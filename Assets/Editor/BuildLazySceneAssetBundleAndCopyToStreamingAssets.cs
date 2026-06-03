using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildTestPrefabAssetBundleAndCopyToStreamingAssets
{
    const string MenuPath = "Build/BuildTestPrefabAssetBundleAndCopyToStreamingAssets";
    const string PrefabAssetPath = "Assets/Prefabs/test.prefab";
    const string AssetBundleName = "test.ab";

    [MenuItem(MenuPath)]
    public static void Execute()
    {
        if (!File.Exists(PrefabAssetPath))
        {
            Debug.LogError($"[BuildTestPrefabAssetBundleAndCopyToStreamingAssets] Prefab not found: {PrefabAssetPath}");
            return;
        }

        var projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
        var outputDir = Path.Combine(projectRoot, "Temp", "LeanCLR", "AssetBundles");
        Directory.CreateDirectory(outputDir);

        var builds = new[]
        {
            new AssetBundleBuild
            {
                assetBundleName = AssetBundleName,
                assetNames = new[] { PrefabAssetPath }
            }
        };

        var target = EditorUserBuildSettings.activeBuildTarget;
        var manifest = BuildPipeline.BuildAssetBundles(outputDir, builds, BuildAssetBundleOptions.None, target);
        if (manifest == null)
        {
            Debug.LogError("[BuildTestPrefabAssetBundleAndCopyToStreamingAssets] BuildAssetBundles failed.");
            return;
        }

        Directory.CreateDirectory(Application.streamingAssetsPath);
        var srcPath = Path.Combine(outputDir, AssetBundleName);
        var dstPath = Path.Combine(Application.streamingAssetsPath, "test.ab.bytes");

        if (!File.Exists(srcPath))
        {
            Debug.LogError($"[BuildTestPrefabAssetBundleAndCopyToStreamingAssets] Built bundle not found: {srcPath}");
            return;
        }

        File.Copy(srcPath, dstPath, true);
        AssetDatabase.Refresh();
        Debug.Log($"[BuildTestPrefabAssetBundleAndCopyToStreamingAssets] Copied {srcPath} to {dstPath}");
    }
}
