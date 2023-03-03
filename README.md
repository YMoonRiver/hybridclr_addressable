# 新增GameFramework Addressables 改版
# 新增循环列表

### UnityGameFramework  

**此框架参考:**   

`GameFramework`：[https://github.com/EllanJiang/GameFramework](https://github.com/EllanJiang/GameFramework)  
`UniRx`: [https://github.com/neuecc/UniRx](https://github.com/neuecc/UniRx)  
`UniTask`: [https://github.com/Cysharp/UniTask](https://github.com/Cysharp/UniTask)   

---

### Demo
`XLua Demo`实现: [https://github.com/coding2233/UnityGameFramework-xLua](https://github.com/coding2233/UnityGameFramework-xLua)  
`ILRuntime`为老版实现，需要在当前仓库切换到`ILRuntime`分支

---

### Package

`Addressable` `1.17.17+`


### 内置模块介绍

---

### DataTable
DataTable为了配置修改不再动态生成或修改对应的序列化的类，进行全动态的解析。
为了做到通用性解析，配置表有一定的规则，以Excel操作的配置表为例。
* 每一行前面第一列带`#`号代表忽略。前面四行是固定的
* 第一行是配置表的名称;
* 第二行是配置表的键值;
* 第三行为当前列的数据类型,`[bool,int,long,float,double,string,Vector2,Vector3,Color]`,`Vector2`示例`100,100`,`Color`示例`#F0F`或者`#FF00FF`;
* 第四行是每一列的说明。
* 实际数据以第五行,第二列开始，第二列的数据一定为`int`类型的唯一识别`id`
* 最后用excel导出为`Unicode 文本`格式即可。

 配置表示例 

|||||||||  
|-|-|-|-|-|-|-|-|  
|#|关卡配置表|  
|#|Id|LevelSort|UIFormId|LevelName|LevelDesc|Leveldubing|IsHide|
|#|int|int|int|string|string|int|bool|
|#|关卡Id|关卡排序|界面编号|关卡名称|关卡描述|配音|是否隐藏|
||20010001|1|300|测试名称|测试说明|2352|false|
||20010002|2|200|测试名称02|测试说明02|23521|false|

使用示例

```csharp
//DataTable加载事件监听
GameMode.Event.AddListener<LoadDataTableEventArgs>(OnLoadDataTable);
//加载DataTable
GameMode.DataTable.LoadDataTable("Assets/Game/DataTable/GameCheckpoint.txt");

//DataTable加载事件回调
private void OnLoadDataTable(object sender,IEventArgs e)
{
    LoadDataTableEventArgs ne = e as LoadDataTableEventArgs;
    if(ne!=null)
    {
        IDataTable idt =  ne.Data;

        TableData td=idt[20010012]["UIFormId"];
        int uiFormId = (int)td;
        
        Debug.Log($"#################################:{ne.Message}");
        foreach (var item in idt)
        {
            Debug.Log($"-------------------------------------------------------");
            TableData td02 = idt[item];
            foreach (var item02 in td02)
            {
                Debug.Log(item02.ToString());
            }
        }
    }
}

//使用已加载的DataTable
IDataTable idt = GameMode.DataTable.GetDataTable("Assets/Game/DataTable/GameCheckpoint.txt");

```
`IDataTable`是当前配置表的所有数据的集合，可使用`foreach`获取到数据的`key`值，`TableData`是具体数据存储对象，主要支持上述的第三行的基本类型。


#### 一、事件模块 `EventManager`

整个框架以事件作为驱动，以达到各个功能之间的解耦效果。除了可以自定义扩展事件以外，框架中还会自带一些事件，后面再详细列表。

1. 新建事件，新建一个类并继承`GameEventArgs`

```csharp
/// <summary>
/// 场景加载中事件
/// </summary>
public class SceneLoadingEventArgs : GameEventArgs<SceneLoadingEventArgs>
{
    /// <summary>
    /// 场景名称
    /// </summary>
    public string SceneName;
    /// <summary>
    /// 场景加载进度
    /// </summary>
    public float Progress;
}
```

2. 订阅事件

```csharp
GameFrameworkMode.GetModule<EventManager>().AddListener<SceneLoadingEventArgs>(OnSceneLoadingCallbak);
```

3. 取消事件订阅

```csharp
GameFrameworkMode.GetModule<EventManager>().RemoveListener<SceneLoadingEventArgs>(OnSceneLoadingCallbak);
```

4. 事件回调的函数实现

```csharp
private void OnSceneLoadingCallbak(object sender, IEventArgs e)
{
    SceneLoadingEventArgs ne = (SceneLoadingEventArgs) e;
    //...
}
```

5. 事件触发

```csharp
//第一种方式 带参数触发事件
GameFrameworkMode.GetModule<EventManager>()
	        .Trigger(this, new SceneLoadingEventArgs() {SceneName = "xxx", Progress = 0.85f});
//第二种方式 不带参数触发事件， 不带参数， 就不用生成新的对象，会直接传null
// GameFrameworkMode.GetModule<EventManager>().Trigger<SceneLoadingEventArgs>(this);
```

---

#### 二、游戏状态模块 `GameStateManager`

游戏状态是整个项目的核心逻辑，建议将所有的逻辑都写再状态之中。增加状态管理几乎将各个类型的项目的开发都能尽量模式话，常用的状态:版本更新状态->配置加载状态->资源预加载->开始游戏->...

1. 增加状态，所有的状态都需要继承GameState,并在类名上添加类标记[GameState]

```csharp
[GameState]
public class LoadConfigState : GameState
{
    /// <summary>
    /// 初始化 -- 只执行一次
    /// </summary>
    public override void OnInit()
    {
        base.OnInit();
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    /// <param name="parameters">不确定参数</param>
    public override void OnEnter(params object[] parameters)
    {
        base.OnEnter(parameters);
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
    }

    /// <summary>
    /// 固定帧函数
    /// </summary>
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    /// <summary>
    /// 渲染帧函数
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
```

2. 状态的类标记有四种类似 

* `[GameState]` 普通状态
* `[GameState(GameStateType.Normal)]` 普通状态
* `[GameState(GameStateType.Ignore)]` 忽略状态，表示在状态管理中忽略这个类的存在
* `[GameState(GameStateType.Start)]` 开始状态，在运行时，第一个运行的状态类标记

3. 状态切换,每个状态都有一个ChangeState函数

```csharp
//切换到开始状态
ChangeState<StartState>();
```

---

#### 三、资源管理模块 `ResourceManager`

资源加载使用`async-await`来做异步加载资源

1. 资源加载(异步加载 )

```csharp
//加载普通资源
TextAsset textAsset= await GameFrameworkMode.GetModule<ResourceManager>().LoadAsset<TextAsset>("datatable","Assets/TextAssets/test.txt");
//实例化GameObject
GameObject obj = await GameFrameworkMode.GetModule<ResourceManager>().LoadAsset<GameObject>("player","Assets/Players/player.prefab");
GameObject player = Instantiate(obj);
```

2. 资源加载(同步加载)

```csharp
//先加载assetbundle
GameFrameworkMode.GetModule<ResourceManager>().LoadAssetBundle("hotfix");
//再加载资源
GameFrameworkMode.GetModule<ResourceManager>().LoadAssetSync("hotfix","main");
```

3. 内置对象池

* 添加预设
```csharp
GameFrameworkMode.GetModule<ResourceManager>().AddPrefab("player","Assets/Prefab/Player.prefab",
					new PoolPrefabInfo() {Prefab = playerPrefab,PreloadAmount=3, MaxAmount = 10});
```
* 生成对象
```csharp
GameObject player= GameFrameworkMode.GetModule<ResourceManager>().Spawn("Assets/Prefab/Player.prefab");
```
* 销毁对象
```csharp
GameFrameworkMode.GetModule<ResourceManager>().Despawn(player);
```

4. 加载场景,场景只支持异步加载

```csharp
AsyncOperation asyncOperation= await GameFrameworkMode.GetModule<ResourceManager>().LoadSceneAsync("mainscene","Assets/Scene/Main.unity");
```

5. 支持编辑器内资源的直接读取和AssetBundle资源读取两种方式的一键切换，避免测试的时候需要重复的打包AssetBundle资源

---

#### 四、UI管理模块 `UIManager`

1. 新建ui预设，新建ui类，继承类`UIView`,绑定并在类名上标明预设的资源路径

```csharp
[UIView("ui","Assets/Prefab/UI/LoadingView.prefab")]
public class LoadingUIView : UIView
{
	/// <summary>
	/// 打开界面
	/// </summary>
	/// <param name="parameters">不确定参数</param>
	public override void OnEnter(params object[] parameters)
	{
		throw new System.NotImplementedException();
	}
	/// <summary>
	/// 退出界面
	/// </summary>
	public override void OnExit()
	{
		throw new System.NotImplementedException();
	}
	/// <summary>
	/// 暂停界面
	/// </summary>
	public override void OnPause()
	{
		throw new System.NotImplementedException();
	}
	/// <summary>
	/// 恢复界面
	/// </summary>
	public override void OnResume()
	{
		throw new System.NotImplementedException();
	}
}
```

2. 打开ui

```csharp
GameFrameworkMode.GetModule<UIManager>().Push<LoadingUIView>();
```

3. 关闭ui,在看到`push`&`pop`的时候，就知道`UIManager`是基于堆栈管理`UI`的，`pop`自然关闭的是最新打开的界面
也可以调用close接口

```csharp
GameFrameworkMode.GetModule<UIManager>().Pop();
```

---

#### 五、数据节点模块 `NodeManager`

数据节点只用来存储在运行中的数据,用法类似`PlayerPrefs`

1. 存数据

```csharp
GameFrameworkMode.GetModule<NodeManager>().SetInt("Level", 10);
```

2. 取数据

```csharp
int level= GameFrameworkMode.GetModule<NodeManager>().GetInt("Level");
```

---

#### 六、Http网页请求模块 `WebRequestManager`

网页请求目前主要包含读取http上的文本文件和下载http服务器上的资源到本地两大功能

1. 请求文本

```csharp
//请求文本
GameFrameworkMode.GetModule<WebRequestManager>().RequestText("http://nothing.com/AssetVersion.txt");
```

2. 请求下载

```csharp

```

3. 事件监听

```csharp
//监听文本请求成功
GameFrameworkMode.GetModule<EventManager>().AddListener<HttpReadTextSuccessEventArgs>(OnHttpReadTextSuccess);
//文本请求失败
GameFrameworkMode.GetModule<EventManager>().AddListener<HttpReadTextFaileEventArgs>(OnHttpReadTextFaile);
//文件下载成功
GameFrameworkMode.GetModule<EventManager>().AddListener<DownloadSuccessEventArgs>(OnDownloadSuccess);
//文件下载失败
GameFrameworkMode.GetModule<EventManager>().AddListener<DownloadFaileEventArgs>(OnDownloadFaile);
//文件下载进度
 GameFrameworkMode.GetModule<EventManager>().AddListener<DownloadProgressEventArgs>(OnDownloadProgress);
```

---  

#### 七、音频管理器模块 `AudioManager`  

统一的声音播放管理，支持默认的背景音乐、ui音效、其他音效已经物体绑定的AudioSource多种模式，以下以播放ui音效为例

1. 添加ui音效音频

```csharp
GameFrameworkMode.GetModule<AudioManager>().AddUISound("soundclip","Assets/Audio/UI/default.wav");
```  

2. 播放ui音效

```csharp
GameFrameworkMode.GetModule<AudioManager>().PlayUISound("soundclip","Assets/Audio/UI/default.wav");
```

3. 停止ui音效,默认停止当前正在播放的音频

```csharp
GameFrameworkMode.GetModule<AudioManager>().StopUISound();
```  

4. 移除ui音效音频

```csharp
GameFrameworkMode.GetModule<AudioManager>().RemoveUISound("Assets/Audio/UI/default.wav");
```  

---  

#### 八、本地化管理模块 `LocalizationManager`

将配置文件中的本地化文件，读取语言存为字典保存在`LocalizationManager`中，使用`LocalizationText`绑定在`UGUI`的`Text`组件上。
同时支持动态设置

```csharp
go.GetComponent<LocalizationText>().Text="GameName";
```  

---

#### 九、设置模块 `SettingMangaer`

默认封装`PlayerPrefs`,使用方法类似。同时添加了`SetQuality`&`SetAllSoundVolume`&`SetBackgroundMusicVolume`&`SetUISoundVolume`&`SetSoundEffectVolume`等默认的设置  
具体使用`GameFrameworkMode.GetModule<SettingMangaer>()`一目了然  

---  

#### 十、网络模块 `NetworkManager`

正在增加中，首先会封装局域网内的连接通信，互联网后面增加。目前使用`kcp`将udp转为可靠传输，传输协议使用`Protobuf`

---

### 内置工具

---

#### 一、AssetBundle打包工具

* 打包工具兼容unity自身右下角的assetbundle的标签设计
* 工具栏在`Tools/AssetBundles Options`,快捷键为ctrl+shift+o
* 打包当前平台`Tools/Build AssetBundles`,快捷键为ctrl+shift+T
* 打包多个平台`Tools/Build AssetBundles Targets`,快捷键为ctrl+shift+Y

二、Addressable打包工具
* 工具栏`AddressableMenu/...`,一键打包，一键增量打包
* 工具栏`Tools/Asset Management/...`

---

### 编辑器扩展

#### Game Module编辑扩展
* 继承`ModuleEditorBase`
* 构造函数同`ModuleEditorBase`
* 在类上添加标记`CustomModuleEditor`


# HybridCLR 体验项目

一个示例热更新项目。

想了解更多，请加 QQ群: 

- HybridCLR c#热更新 开发交流群：651188171
- HybridCLR 新手群：428404198

你可以使用发布的包来体验HybridCLR热更新功能。

**示例项目使用 Unity 2020.3.33(任意后缀子版本如f1、f1c1、f1c2都可以) 版本**，2019.4.x、2020.3.x、2021.3.x系列都可以，但为了避免新手无谓的出错，尽量使用2020.3.33版本来体验。

## 目录介绍

- Assets Unity项目目录
  - Main AOT主包模块，对应常规项目的主项目，资源更新模块
  - 剩下代码在默认的全局Assembly-Csharp.dll中，均为热更新脚本
- HybridCLRData 包含HybridCLR的il2cpp本地安装目录
- Packages/com.focus-creative-games/hybridclr_unity 为HybridCLR for Unity包。暂时先作为local package，成熟后做成独立package。

## 使用介绍

HybridCLR为c++实现，只有打包后才可使用。日常开发在编辑器下，无需打包。

如何打包出一个可热更新的包，请先参阅 [快速开始](https://focus-creative-games.github.io/hybridclr/start_up/)。


## 运行流程

本示例演示了如下几部分内容

- 将dll和资源打包成ab
- 热更新脚本挂载到热更新资源中，并且正常运行
- 直接反射运行普通热更新函数App::Main

进入场景后，Main场景中的LoadDll会按顺序加载StreamingAssets目录下common AssetBundle里的Assembly-Csharp.dll。接着运行App::Main函数。

## 体验热更新

### 通用预备工作

**===> 安装必须的Unity版本**

根据你所使用的Unity年度版本，**还需要额外**安装2019.4.40、2020.3.33或者2021.3.1版本（必须包含il2cpp模块），不限 f1、f1c1之类后缀。

**再次提醒** 当前Unity版本必须安装了 il2cpp 组件。如果未安装，请自行在UnityHub中安装。新手自行google或百度。

**===> 安装 visual studio**

要求必须安装 `使用c++的游戏开发` 这个组件

**===> 安装git**

### Unity版本相关特殊操作

**===> Unity 2021**

对于使用Unity 2021版本（2019、2020不需要）打包iOS平台的开发者，由于HybridCLR需要裁减后的AOT dll，但Unity Editor未提供公开接口可以复制出target为iOS时的AOT dll，故必须使用修改后的UnityEditor.CoreModule.dll覆盖Unity自带的相应文件。

具体操作为将 `HybridCLRData/ModifiedUnityAssemblies/2021.3.1/UnityEditor.CoreModule-{Win,Mac}.dll` 覆盖 `{Editor安装目录}/Editor/Data/Managed/UnityEngine/UnityEditor.CoreModule`，具体覆盖目录有可能因为操作系统或者Unity版本而有不同。

**由于权限问题，该操作无法自动完成，需要你手动执行。**

这个 UnityEditor.CoreModule.dll 每个Unity小版本都不相同，我们目前暂时只提供了2021.3.1版本，如需其他版本请自己手动制作，详情请见 [修改UnityEditor.CoreModule.dll](https://focus-creative-games.github.io/hybridclr/modify_unity_dll/)

**===> Unity 2019**

为了支持2019，需要修改il2cpp生成的源码，因此我们修改了2019版本的il2cpp.exe工具。故Installer的安装过程多了一个额外步骤：将 `{package}/Data/ModifiedUnityAssemblies/2019.4.40/Unity.IL2CPP.dll` 复制到 `{package}/Data/LocalIl2CppData/il2cpp/build/deploy/net471/Unity.IL2CPP.dll`

**注意，该操作自动完成，不需要手动操作。**

### 配置

**===> 确保[com.focus-creative-games.hybridclr_unity](https://github.com/focus-creative-games/hybridclr_unity) package已经正确安装**

为了使用HybridCLR，需要安装hybridclr_unity插件。 不熟悉从url安装package的请看 [install from git](https://docs.unity3d.com/Manual/upm-ui-giturl.html)。

由于网络原因，在unity中可能无法安装成功。你可以先把 [com.focus-creative-games.hybridclr_unity](https://github.com/focus-creative-games/hybridclr_unity) clone或者下载到本地，然后再 [install from disk](https://docs.unity3d.com/Manual/upm-ui-local.html)


目前需要几个配置文件

**===> HybridCLRGlobalSettings.asset**

HybridCLR全局配置，单例。 trial项目已经创建。新项目请在 Unity Editor的 Project 窗口右键 `HybridCLR/GlobalSettings` 创建。

关键参数介绍：

- Enable。 是否开启HybridCLR。
- HotUpdateAssmblyDefinitions。 以Assembly Definition形式存在的热更新模块。不能与下面的Hotfix Dlls重复指定模块。
- HotUpdate Dlls。 以dll形式存在的热更新模块。不能与HotUpdateAssmblyDefinitions重复指定模块。

**===> HotUpdateAssemblyManifest.asset**

包含了需要补充元数据的AOT assembly列表。这个字段原来在 HybridCLRGlobalSettings 中。

### 安装及打包及热更新测试

以Win64为例，其他平台同理。

- 安装HybridCLR (安装HybridCLR的原理请看 [快速上手](https://focus-creative-games.github.io/hybridclr/start_up/) )
    - 点击菜单 `HybridCLR/Installer...`，弹出安装界面。
    - 如果安装界面没有错误或者警告，则说明il2cpp路径设置正常，否则需要你手动选择正确的il2cpp目录
    - 点击 install 按钮完成安装
- 打包主工程
  
  **请确保你已经掌握了常规的il2cpp为backend的打包过程**

  **请确保你已经在你电脑上对于一个未使用HybrildCLR的项目成功打包出il2cpp为backend的相应包**，也就是打包环境是正常的！

打包前需要先在 Player Settings里作如下设置：
- script backend 必须选择 il2cpp
- Api Compatibility Level 选择 .NET 4.x（unity 2021 及之后版本这里显示为 .NET framework）
- 关闭 增量式gc 选项 (incremental gc)

打包：
- 菜单 HybridCLR/Build/Win64 ，运行完成后，会在Release_Win64目录下生成程序
- 运行Release_Win64/HybridCLRTrial.exe，会看到打出 hello, HybridCLR.prefab

更新ab包：
  - 修改HotFix项目的PrintHello代码，比如改成打印 "hello,world"。
  - 运行菜单 HybridCLR/BuildBundles/Win64，重新生成ab
  - 将StreamingAssets下的ab包复制到Release_Win64\HybridCLRTrial_Data\StreamingAssets。
  - 再将运行，屏幕上会打印"hello,world"。

### HybridCLR相关Editor菜单介绍

- `Installer...` 打开 安装器
- `GenerateLinkXml` 自动生成热更新代码所需的link.xml。
- `GenerateMethodBridge` 生成桥接函数
- `GenerateAOTGenericReference` 生成热更新模块中用到的AOT泛型实例化
- `GenerateReversePInvokeWrapper` 生成 MonoPInvokeCallbackAttribute的预留桩函数
- `GenerateAll` 生成以上所有 GenerateXXX
- `CompileDll` 编译热更新dll
- `BuildBundles` 构建用于热更资源和代码的ab包
- `Build` 一键打包相关快捷命令

### 其他

剩下的体验之旅，比如各种c#特性，自己体验吧。

