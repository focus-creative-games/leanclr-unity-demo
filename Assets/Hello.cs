using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class Hello : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        StartCoroutine(LoadLazyLoadedDll());
    }

    private readonly Dictionary<string, byte[]> _assets = new Dictionary<string, byte[]>();

    private IEnumerator LoadLazyLoadedDll()
    {
        string dllName = "LazyLoaded.dll.bytes";
        string testPrefabAbName = "test.ab.bytes";
        var assets = new string[] { dllName, testPrefabAbName };
        foreach (var asset in assets)
        {
            string assetPath = $"{Application.streamingAssetsPath}/{asset}";
            if (!assetPath.Contains("://"))
            {
                assetPath = "file://" + assetPath;
            }
            var www = UnityWebRequest.Get(assetPath);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"start download asset:{asset} {assetPath} error:{www.error}");
                yield break;
            }
            _assets[asset] = www.downloadHandler.data;
        }
        LoadAssembly(dllName);
        InstantiatePrefab(testPrefabAbName);
        CallReflectionTest();
    }

    private void LoadAssembly(string dllName)
    {
        byte[] dllBytes = _assets[dllName];
#if !UNITY_EDITOR
        Debug.Log($"load dll bytes length:{dllBytes.Length}");
        Assembly.Load(dllBytes);
        Debug.Log($"load dll:{dllName} success");
#else
        Debug.Log("ignore load dll in editor");
#endif
    }

    private void InstantiatePrefab(string testPrefabAbName)
    {
        // test.prefab 上没有挂载 LazyLoaded中的脚本，因为Unity中要还原ab上挂载的脚本，需要满足一定的条件，目前暂时实现，
        // 但很快将会支持。具体原理见 hybridclr 文档：[MonoBehaviour支持](https://www.hybridclr.cn/docs/basic/monobehaviour)
        // 
        byte[] testPrefabAbBytes = _assets[testPrefabAbName];
        var ab = AssetBundle.LoadFromMemory(testPrefabAbBytes);
        if (ab == null)
        {
            Debug.LogError("load test.ab.bytes failed");
            return;
        }
        var prefab = ab.LoadAsset<GameObject>("test");
        if (prefab == null)
        {
            Debug.LogError("load prefab 'test' from test.ab.bytes failed");
            return;
        }
        Instantiate(prefab);
    }

    private void CallReflectionTest()
    {
        Assembly oazyLoadDll = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "LazyLoaded");
        Type testType = oazyLoadDll.GetType("Test");
        MethodInfo runMethod = testType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static);
        runMethod.Invoke(null, null);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    Debug.Log("Update");
    //}
}
