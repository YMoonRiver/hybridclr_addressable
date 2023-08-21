#if ADDRESSABLES_SUPPORT
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using static UnityEngine.AddressableAssets.Addressables;

namespace Wanderer.GameFramework
{
    public class AddressableVersion : IResourceVersion
    {
        private bool _isCheckUpdate;
        private AsyncOperationHandle<List<string>> _checkHandle;

        private bool useNewIp = true;

        public override async void CheckUpdate(Action<bool> needUpdate)
        {
            string _catalogPath = Application.persistentDataPath + "/com.unity.addressables";
            if (Directory.Exists(_catalogPath))
            {
                try
                {
                    Directory.Delete(_catalogPath, true);
                    Debug.Log("delete catalog cache done!");
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }

            //请求配置文件
            var config = GameFrameworkMode.GetModule<ConfigManager>();
            string configURI = 
                $"{config["ConfigPath"]}/{Application.platform}/DefaultConfig.json";
            Log.Info(configURI);
            var configHandle = GameFrameworkMode.GetModule<WebRequestManager>().RequestText(configURI, null);
            await configHandle;
            Log.Info($"configHandle.Result: {configHandle.Result}");
            config.SetData(configHandle.Result);

            var res = GameFrameworkMode.GetModule<ResourceManager>();
            res.Version.resServerOriIp = config["ResTestUpdatePath"].ToString();
            res.Version.resServerNewIp =
                $"{config["ResOfficialUpdatePath"]}/{Application.platform}";

            //重定向
            Addressables.InternalIdTransformFunc = InternalIdTransformFunc;

            //检查更新信息
            Log.Info("AddressableVersion CheckUpdate");
            var initHandle = InitializeAsync();
            await initHandle.Task;
            if (_isCheckUpdate)
            {
                Addressables.Release(_checkHandle);
                _isCheckUpdate = false;
            }
            Log.Info("CheckForCatalogUpdates");
            _checkHandle = CheckForCatalogUpdates(false);
            _isCheckUpdate = true;
            await _checkHandle.Task;
            Log.Info($"Check Result Count:{_checkHandle.Result.Count}");
            
            foreach (var item in _checkHandle.Result)
            {
                Log.Info($"Check Result :{item}");
            }
            
            needUpdate?.Invoke(_checkHandle.Result.Count > 0);
        }

        /// <summary>
        /// 更新资源
        /// </summary>
        /// <param name="callback">下载回调[进度(0-1)，大小(KB),速度(KB/S),剩余时间(s)]</param>
        /// <param name="downloadComplete">下载完成</param>
        /// <param name="errorCallback">下载错误</param>
        /// <returns></returns>
        public override async void UpdateResource(Action<float, double, double, float> callback, Action downloadComplete, Action<string, string> errorCallback, string label)
        {
            try
            {
                if (_isCheckUpdate)
                {
                    bool hasLabel = !string.IsNullOrEmpty(label);

                    if (_checkHandle.Result.Count > 0)
                    {
                        var updateHandle = Addressables.UpdateCatalogs(_checkHandle.Result, false);
                        await updateHandle.Task;
                        var locators = updateHandle.Result;
                        HashSet<object> downloadKeys = new HashSet<object>();
                        long totalDownloadSize = 0;
                        foreach (var locator in locators)
                        {
                            Log.Info($"Update locator:{locator.LocatorId}");

                            var sizeHandle = Addressables.GetDownloadSizeAsync(locator.Keys);
                            await sizeHandle.Task;
                            long downloadSize = sizeHandle.Result;
                            if (downloadSize > 0)
                            {
                                if (hasLabel)
                                {
                                    foreach (var key in locator.Keys)
                                    {
                                        if (key.ToString().Equals(label))
                                        {
                                            totalDownloadSize += downloadSize;
                                            downloadKeys.Add(key);
                                            Debug.Log($"download key: {key}");
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    totalDownloadSize += downloadSize;
                                    foreach (var key in locator.Keys)
                                    {
                                        downloadKeys.Add(key);
                                        //Log.Info($"locator[{locator}] size:{downloadSize} key:{key}");
                                    }
                                }
                            }
                        }
                        if (totalDownloadSize > 0)
                        {
                            double downloadKBSize = totalDownloadSize / 1024.0f;
                            float downloadStartTime = Time.realtimeSinceStartup;
                            var downloadHandle = Addressables.DownloadDependenciesAsync(downloadKeys, MergeMode.Union);
                            while (!downloadHandle.IsDone)
                            {
                                float percentage = downloadHandle.PercentComplete;
                                double useTime = Time.realtimeSinceStartup - downloadStartTime;
                                double downloadSpeed = ((double)percentage * downloadKBSize) / useTime;
                                float remainingTime = (float)((downloadKBSize / downloadSpeed) / downloadSpeed - useTime);
                                //回调
                                callback?.Invoke(percentage, downloadKBSize, downloadSpeed, remainingTime);
                                await Task.Delay(300);
                            }
                            Addressables.Release(downloadHandle);
                        }
                        downloadComplete?.Invoke();
                        Addressables.Release(updateHandle);
                    }
                    Addressables.Release(_checkHandle);
                    _isCheckUpdate = false;
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(e.Message, e.ToString());
            }
        }

        private string InternalIdTransformFunc(IResourceLocation location)
        {
            //判定是否是一个AB包的请求
            if (location.Data is AssetBundleRequestOptions)
            {
                //PrimaryKey是AB包的名字
                //path就是StreamingAssets/Bundles/AB包名.bundle,其中Bundles是自定义文件夹名字,发布应用程序时,复制的目录
                string InternalId_ = location.InternalId;
                if (useNewIp && InternalId_.Contains(resServerOriIp))
                {
                    InternalId_ = InternalId_.Replace(resServerOriIp, resServerNewIp);
                    InternalId_ = InternalId_.Replace('\\' , '/');
                    Debug.Log("InternalId_" + InternalId_);
                }

                return InternalId_;
            }
            else
            {
                string InternalId_ = location.InternalId;
                if (useNewIp && InternalId_.Contains(resServerOriIp))
                {
                    InternalId_ = InternalId_.Replace(resServerOriIp, resServerNewIp);
                    Debug.Log("@InternalId_" + InternalId_);
                }

                return InternalId_;
            }
        }

    }
}
#endif