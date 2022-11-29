using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Wanderer.GameFramework;

public class LoadDll : MonoBehaviour
{


    public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
    };

    void Start()
    {
        this.StartGame();
    }

    void StartGame()
    {
        LoadMetadataForAOTAssemblies();

#if !UNITY_EDITOR
        TextAsset dllBytes = GameMode.Resource.Asset.LoadAsset<TextAsset>("Assets/Addressable/Hall/Hybird/Assembly-CSharp.dll.bytes");
        var gameAss = System.Reflection.Assembly.Load(dllBytes.bytes);
#else
        var gameAss = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
#endif

        var hotUpdatePrefab = GameMode.Resource.Asset.LoadAsset<GameObject>("Assets/Addressable/Hall/Prefabs/HotUpdatePrefab.prefab");
        GameObject go = Instantiate(hotUpdatePrefab, GameMode.Self.transform);
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        string[] dlls = { "Assets/Addressable/Hall/Hybird/mscorlib.dll.bytes", "Assets/Addressable/Hall/Hybird/System.dll.bytes", "Assets/Addressable/Hall/Hybird/System.Core.dll.bytes" };
        // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
        // 我们在BuildProcessors里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in dlls)
        {
            byte[] dllBytes = GameMode.Resource.Asset.LoadAsset<TextAsset>(aotDllName).bytes;
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }
}
