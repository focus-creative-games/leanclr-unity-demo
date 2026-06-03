using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

/// <summary>
/// Compiles player assemblies via PlayerBuildInterface into Temp/LeanCLR/CompiledDlls/{BuildTarget}, then copies LazyLoad.dll to Resources as a .bytes asset.
/// </summary>
public static class CompileAndCopyLazyLoadDll
{
    const string MenuPath = "Build/CompileAndCopyLazyLoadDllToStreamingAssets";

    [MenuItem(MenuPath)]
    public static void Execute()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        var group = BuildPipeline.GetBuildTargetGroup(target);

        var projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
        var buildTargetFolder = target.ToString();
        var outputDir = Path.Combine(projectRoot, "Temp", "LeanCLR", "CompiledDlls", buildTargetFolder);

        if (Directory.Exists(outputDir))
            Directory.Delete(outputDir, true);
        Directory.CreateDirectory(outputDir);

        var settings = new ScriptCompilationSettings
        {
            target = target,
            group = group,
            options = ScriptCompilationOptions.None,
            subtarget = GetScriptCompilationSubtarget(target),
        };

        var result = PlayerBuildInterface.CompilePlayerScripts(settings, outputDir);
#if UNITY_2022
            UnityEditor.EditorUtility.ClearProgressBar();
#endif
        if (result.assemblies == null)
        {
            Debug.LogError($"[CompileAndCopyLazyLoadDll] Compilation failed for target {target}:");
            return;
        }
        Directory.CreateDirectory(Application.streamingAssetsPath);
        string lazyLoadedDllName = "LazyLoaded.dll";
        string srcLazyLoadDllPath = Path.Combine(outputDir, lazyLoadedDllName);
        string dstLazyLoadDllPath = Path.Combine(Application.streamingAssetsPath, $"{lazyLoadedDllName}.bytes");
        File.Copy(srcLazyLoadDllPath, dstLazyLoadDllPath, true);
        Debug.Log($"copy {srcLazyLoadDllPath} to {dstLazyLoadDllPath}");
    }

    /// <summary>
    /// Standalone targets use EditorUserBuildSettings.standaloneBuildSubtarget; other platforms use 0 (avoids GetActiveSubtargetFor, missing on some Unity versions).
    /// </summary>
    static int GetScriptCompilationSubtarget(BuildTarget target)
    {
        switch (target)
        {
        case BuildTarget.StandaloneWindows:
        case BuildTarget.StandaloneWindows64:
        case BuildTarget.StandaloneOSX:
            return (int)EditorUserBuildSettings.standaloneBuildSubtarget;
        default:
            return 0;
        }
    }
}
