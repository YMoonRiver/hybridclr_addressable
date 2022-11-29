using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 选定图集文件夹
/// </summary>
public class AutoSetAtlasContent : Editor
{
    [MenuItem("Tools/AutoSetAtlas _F3")]
    static void AutoSetAtlasContents()
    {
        var arr = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);
        string folder = AssetDatabase.GetAssetPath(arr[0]);
        DirectoryInfo dirInfo = new DirectoryInfo(folder);
        if (!Directory.Exists(folder))
        {
            Debug.LogError("不存在目录");
            return;
        }
        string _texturePath = folder;
        string atlasName = dirInfo.Name;
        string _atlasPath = $"Assets/Addressable/Res/Atlas/{atlasName}.spriteatlas";

        SpriteAtlas atlas = new SpriteAtlas();
        // 设置参数 可根据项目具体情况进行设置
        SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
        {
            blockOffset = 1,
            enableRotation = false,
            enableTightPacking = false,
            padding = 2,
        };
        atlas.SetPackingSettings(packSetting);

        SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
        {
            readable = false,
            generateMipMaps = false,
            sRGB = true,
            filterMode = FilterMode.Bilinear,
        };
        atlas.SetTextureSettings(textureSetting);

        TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings
        {
            name = "Standalone",
            maxTextureSize = 2048,
            format = TextureImporterFormat.DXT5
        };
        atlas.SetPlatformSettings(textureImporterPlatformSettings);
        textureImporterPlatformSettings = new TextureImporterPlatformSettings
        {
            name = "iPhone",
            maxTextureSize = 2048,
            format = TextureImporterFormat.ASTC_HDR_4x4,
            overridden = true,
        };
        atlas.SetPlatformSettings(textureImporterPlatformSettings);
        textureImporterPlatformSettings = new TextureImporterPlatformSettings
        {
            name = "Android",
            maxTextureSize = 2048,
            format = TextureImporterFormat.ETC2_RGBA8,
            overridden = true,
        };
        atlas.SetPlatformSettings(textureImporterPlatformSettings);

        AssetDatabase.CreateAsset(atlas, _atlasPath);

        //// 1、添加文件
        //DirectoryInfo dir = new DirectoryInfo(_texturePath);
        //// 这里我使用的是png图片，已经生成Sprite精灵了
        //FileInfo[] files = dir.GetFiles("*.png");
        //foreach (FileInfo file in files)
        //{
        //    atlas.Add(new[] { AssetDatabase.LoadAssetAtPath<Sprite>($"{_texturePath}/{file.Name}") });
        //}

        // 2、添加文件夹
        Object obj = AssetDatabase.LoadAssetAtPath(_texturePath, typeof(Object));
        atlas.Add(new[] { obj });

        AssetDatabase.SaveAssets();
    }

    public static string FormatFilePath(string filePath)
    {
        var path = filePath.Replace('\\', '/');
        path = path.Replace("//", "/");
        return path;
    }
}