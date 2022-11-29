using HybridCLR.Editor.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HybridCLR.Editor
{
    public static class BuildAssetsCommand
    {
        public static string HybridCLRBuildCacheDir => Application.dataPath + "/HybridCLRBuildCache";

        public static string AssetBundleOutputDir => $"{HybridCLRBuildCacheDir}/AssetBundleOutput";

        public static string AssetBundleSourceDataTempDir => $"{HybridCLRBuildCacheDir}/AssetBundleSourceData";


        public static string GetAssetBundleOutputDirByTarget(BuildTarget target)
        {
            return $"{AssetBundleOutputDir}/{target}";
        }

        public static string GetAssetBundleTempDirByTarget(BuildTarget target)
        {
            return $"{AssetBundleSourceDataTempDir}/{target}";
        }

        public static string ToRelativeAssetPath(string s)
        {
            return s.Substring(s.IndexOf("Assets/"));
        }

        [MenuItem("HybridCLR/Build/BuildAssetsAndCopyToAddressable")]
        public static void BuildAndCopyAOTHotUpdateDlls()
        {
            CompileDllCommand.CompileDllActiveBuildTarget();
            CopyAOTAssembliesToAddressable();
            CopyHotUpdateAssembliesToAddressable();
            AssetDatabase.Refresh();
        }

        public static void CopyAOTAssembliesToAddressable()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string assetHybirdPathDst = $"{Application.dataPath}/Addressable/Hall/Hybird";
            Directory.CreateDirectory(assetHybirdPathDst);

            foreach (var dll in LoadDll.AOTMetaAssemblyNames)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                string dllBytesPath = $"{assetHybirdPathDst}/{dll}.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                Debug.Log($"[CopyAOTAssembliesToAddressable] copy AOT dll {srcDllPath} -> {dllBytesPath}");
            }
        }

        public static void CopyHotUpdateAssembliesToAddressable()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;

            string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            //string hotfixAssembliesDstDir = Application.streamingAssetsPath;
            string assetHybirdPathDst = $"{Application.dataPath}/Addressable/Hall/Hybird";
            Directory.CreateDirectory(assetHybirdPathDst);

            foreach (var dll in SettingsUtil.HotUpdateAssemblyFiles)
            {
                string dllPath = $"{hotfixDllSrcDir}/{dll}";
                string dllBytesPath = $"{assetHybirdPathDst}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                Debug.Log($"[CopyHotUpdateAssembliesToAddressable] copy hotfix dll {dllPath} -> {dllBytesPath}");
            }
        }

        public static void BuildAssetBundleByTarget(BuildTarget target, bool buildAot)
        {
            CompileDllCommand.CompileDll(target);
            if (buildAot)
            {
                CopyAOTAssembliesToAddressable();
            }
            CopyHotUpdateAssembliesToAddressable();
            AssetDatabase.Refresh();
        }

        public static void BuildSceneAssetBundleActiveBuildTarget()
        {
            BuildAssetBundleByTarget(EditorUserBuildSettings.activeBuildTarget, true);
        }

        public static void BuildSceneAssetBundleActiveBuildTargetExcludeAOT()
        {
            BuildAssetBundleByTarget(EditorUserBuildSettings.activeBuildTarget, false);
        }

    }
}
