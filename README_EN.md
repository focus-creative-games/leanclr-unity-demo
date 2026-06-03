# leanclr4unity_demo

Sample project for [leanclr4unity](https://github.com/focus-creative-games/leanclr4unity). Entry scene: `main`. Main logic: `Assets/Hello.cs`.

## What it demonstrates

1. **LeanCLR instead of IL2CPP** — LeanCLR is enabled (`Project Settings → LeanCLR`) for player builds; scripts and AOT go through LeanCLR, not IL2CPP.
2. **Script restoration on scenes & AssetBundles** — `MonoBehaviour` from assemblies in the main build work on scenes and ABs (e.g. `Print` on `test.prefab`).
3. **Lazy loaded assembly** — `LazyLoaded` is not in the main package; at runtime load `LazyLoaded.dll.bytes` from `StreamingAssets`, then call `Test.Run` via reflection.
4. **Limitation** — **Do not** put scripts from lazy assemblies on scenes, prefabs, or AssetBundles; you will get Missing Script. See [HybridCLR — MonoBehaviour support](https://www.hybridclr.cn/docs/basic/monobehaviour).

## Usage

**Play in Editor**

1. Open `Assets/Scenes/main.unity`.
2. If `StreamingAssets/*.bytes` are missing, run:
   - `Build → CompileAndCopyLazyLoadDllToStreamingAssets`
   - `Build → BuildTestPrefabAssetBundleAndCopyToStreamingAssets`
3. Press Play. The Editor skips `Assembly.Load` for the DLL, but AB loading and reflection still run.

**Player build**

1. Select the target platform in Build Settings.
2. Run the two Build menu items above (per platform).
3. Build (tested on WebGL and Win64).

## Links

- [leanclr4unity](https://github.com/focus-creative-games/leanclr4unity)
