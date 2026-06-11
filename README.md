# leanclr-unity-demo

[leanclr-unity](https://github.com/focus-creative-games/leanclr-unity) 的示例项目。入口场景 `main`，主逻辑见 `Assets/Hello.cs`。

## 演示内容

1. **LeanCLR 替代 IL2CPP**：打包时启用 LeanCLR（`Project Settings → LeanCLR`），由 LeanCLR 处理脚本与 AOT，不再走 IL2CPP。
2. **场景与 AB 脚本还原**：主包程序集里的 `MonoBehaviour` 可正常挂在场景和 AssetBundle 上（示例：`test.prefab` 上的 `Print`）。
3. **Lazy Loaded Assembly**：`LazyLoaded` 不进主包；运行时从 `StreamingAssets` 加载 `LazyLoaded.dll.bytes`，再反射调用 `Test.Run`。

## **限制**

- **不要**在场景、Prefab、AB 等资源上挂载 lazy 程序集里的脚本，否则会出现 Missing Script。原理见 [HybridCLR — MonoBehaviour 支持](https://www.hybridclr.cn/docs/basic/monobehaviour)。
- 如果Lazy Loaded也有部分代码被编译到aot（即没有在aot.xml中对该程序集禁用aot），则要求加载Lazy Loaded程序集必须跟构建过程中的生成的裁剪后的aot dll完全一致。因此并不能直接使用 Compile Dll生成的程序集，必须使用打包时生成的裁剪后的aot dll。如果你有hybridclr使用经验，可以理解为lazy loaded程序集必须来自打包过程中生成的AssemblyPostStripped目录下的dll。目前leanclr-unity并没有主动复制构建过程中生成的aot dll，因此需要手动从 `Library/Bee/artifacts` 目录下查找并复制。我们很快会彻底解决这个问题。

## 使用

**Editor 试玩**

1. 打开 `Assets/Scenes/main.unity`。
2. 若无 `StreamingAssets` 下的 `.bytes`，先执行：
   - `Build → CompileAndCopyLazyLoadDllToStreamingAssets`
   - `Build → BuildTestPrefabAssetBundleAndCopyToStreamingAssets`
3. 点 Play。Editor 下不会 `Assembly.Load` DLL，但可测 AB 与反射。

**打 Player 包**

1. 选好 Build Settings 里的目标平台。
2. 执行上面两个 Build 菜单（需与目标平台一致）。
3. Build（已验证 WebGL、Win64）。

## 链接

- [leanclr-unity](https://github.com/focus-creative-games/leanclr-unity)
