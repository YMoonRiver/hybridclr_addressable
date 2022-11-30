using HybridCLR.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 选中文件夹，以文件夹为单位创建组
/// 打包菜单
/// </summary>
public class AddressableUtil
{
    public static string FormatFilePath(string filePath)
    {
        var path = filePath.Replace('\\', '/');
        path = path.Replace("//", "/");
        return path;
    }

    #region 打包设置
    public static string GetServerDataPath()
    {
        var path = Application.dataPath.Replace("Assets", "ServerData");
        path = FormatFilePath(path);
        return path;
    }

    //[MenuItem("AddressableMenu/Clean Content And Folder", priority = 2)] //清理上次打包的资源包括服务器热更新数据
    public static void ClearAllAddressBuild()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetSettings.CleanPlayerContent(settings.ActivePlayerDataBuilder);
        var serverDataPath = AddressableAssetSettingsDefaultObject.Settings.RemoteCatalogBuildPath.GetValue(AddressableAssetSettingsDefaultObject.Settings);
        Debug.Log("clear serverdata " + serverDataPath);
        if (System.IO.Directory.Exists(serverDataPath))
        {
            System.IO.Directory.Delete(serverDataPath, true);
        }
    }

    /// <summary>
    /// 重新构建Address资源
    /// </summary>
    [MenuItem("AddressableMenu/Clean And Build Content", priority = 3)]
    public static void ReBuildAddress()
    {
        ClearAllAddressBuild();
        //2.对所有资源进行重新分组打标签(TODO)
        //LoopSetAllDirectorToAddress(GameSettings.GetABRootPath());
        BuildAssetsCommand.BuildSceneAssetBundleActiveBuildTarget();
        AddressableAssetSettings.BuildPlayerContent();
        
    }

    /// 对比更新列表
    //[MenuItem("AddressableMenu/GatherModifiedEntries And Creat Update Group", priority = 4)]
    public static void CheckForUpdateContent()
    {
        //与上次打包做资源对比 to get the path of the bin file automatically.
        string buildPath = ContentUpdateScript.GetContentStateDataPath(false);
        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        List<AddressableAssetEntry> entrys = ContentUpdateScript.GatherModifiedEntries(m_Settings, buildPath);
        if (entrys.Count == 0)
        {
            Debug.Log("没有资源更新");
            return;
        }
        StringBuilder sbuider = new StringBuilder();
        sbuider.AppendLine("Need Update Assets:");
        foreach (var entry in entrys)
        {
            sbuider.AppendLine(entry.address);
        }
        Debug.Log(sbuider.ToString());
        //将被修改过的资源单独分组
        var groupName = string.Format("UpdateGroup_{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
        ContentUpdateScript.CreateContentUpdateGroup(m_Settings, entrys, groupName);
    }

    //迭代打包
    [MenuItem("AddressableMenu/Build Update Content", priority = 5)]
    public static void BuildUpdate()
    {
        BuildAssetsCommand.BuildSceneAssetBundleActiveBuildTargetExcludeAOT();

        //对比更新列表
        CheckForUpdateContent();

        var path = ContentUpdateScript.GetContentStateDataPath(false);
        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        AddressablesPlayerBuildResult result = ContentUpdateScript.BuildContentUpdate(AddressableAssetSettingsDefaultObject.Settings, path);
        Debug.Log("BuildFinish path = " + m_Settings.RemoteCatalogBuildPath.GetValue(m_Settings));
       
    }

    //打印构建路径
    [MenuItem("AddressableMenu/Print Path", priority = 7)]
    public static void Test()
    {
        Debug.Log("BuildPath = " + Addressables.BuildPath);
        Debug.Log("PlayerBuildDataPath = " + Addressables.PlayerBuildDataPath);
        Debug.Log("RemoteCatalogBuildPath = " + AddressableAssetSettingsDefaultObject.Settings.RemoteCatalogBuildPath.GetValue(AddressableAssetSettingsDefaultObject.Settings));
    }

    [MenuItem("AddressableMenu/Assets To Addressables")]
    public static void SetAddressablesAssets()
    {
        Wanderer.GameFramework.AddressablesEditor.SetAddressablesAssets();
    }

    [MenuItem("AddressableMenu/Asset Group #&G")]
    private static void OpenWindow()
    {
        Wanderer.GameFramework.AssetGroupEditor.OpenWindow();
    }
    #endregion
}
