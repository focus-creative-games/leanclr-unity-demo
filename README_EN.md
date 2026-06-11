# leanclr-unity-demo

Sample project for [leanclr-unity](https://github.com/focus-creative-games/leanclr-unity). Entry scene: `main`. Main logic: `Assets/Hello.cs`.

## What it demonstrates

1. **LeanCLR instead of IL2CPP** — Enable LeanCLR for player builds (`Project Settings → LeanCLR`); LeanCLR handles scripts and AOT instead of IL2CPP.
2. **Script restoration on scenes & AssetBundles** — `MonoBehaviour` from main-package assemblies can be used on scenes and AssetBundles (e.g. `Print` on `test.prefab`).
3. **Lazy loaded assembly** — `LazyLoaded` is not in the main package; at runtime load `LazyLoaded.dll.bytes` from `StreamingAssets`, then call `Test.Run` via reflection.

## Limitations

- **Do not** attach scripts from lazy assemblies to scenes, prefabs, AssetBundles, or other assets — you will get Missing Script. See [HybridCLR — MonoBehaviour support](https://www.hybridclr.cn/docs/basic/monobehaviour).
- If part of a Lazy Loaded assembly is still compiled to AOT (i.e. AOT is not disabled for that assembly in `aot.xml`), the DLL loaded at runtime must match exactly the **stripped AOT DLL** produced during the build. You cannot use the assembly from **Compile Dll** directly; you must use the stripped AOT DLL from the player build. With HybridCLR experience: treat it like lazy loaded assemblies must come from the `AssemblyPostStripped` DLLs under the build output. leanclr-unity does not copy those build-time AOT DLLs for you yet — copy them manually from `Library/Bee/artifacts`. This will be automated soon.

## Usage

**Play in Editor**

1. Open `Assets/Scenes/main.unity`.
2. If `StreamingAssets/*.bytes` are missing, run:
   - `Build → CompileAndCopyLazyLoadDllToStreamingAssets`
   - `Build → BuildTestPrefabAssetBundleAndCopyToStreamingAssets`
3. Press Play. The Editor does not `Assembly.Load` the DLL, but you can still test AB loading and reflection.

**Player build**

1. Select the target platform in Build Settings.
2. Run the two Build menu items above (must match the target platform).
3. Build (verified on WebGL and Win64).

## Links

- [leanclr-unity](https://github.com/focus-creative-games/leanclr-unity)
