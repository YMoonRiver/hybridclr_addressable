using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

class UniWebViewEditorSettings: ScriptableObject
{
    const string assetPath = "Assets/Editor/UniWebView/settings.asset";

    [SerializeField]
    internal bool usesCleartextTraffic = false;

    [SerializeField]
    internal bool writeExternalStorage = false;

    [SerializeField]
    internal bool accessFineLocation = false;

    [SerializeField]
    internal bool addsKotlin = true;

    [SerializeField]
    internal bool addsAndroidBrowser = true;


    internal static UniWebViewEditorSettings GetOrCreateSettings() {
        var settings = AssetDatabase.LoadAssetAtPath<UniWebViewEditorSettings>(assetPath);

        if (settings == null) {
            settings = ScriptableObject.CreateInstance<UniWebViewEditorSettings>();

            Directory.CreateDirectory("Assets/Editor/UniWebView/");
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
        }

        return settings;
    }

    internal static SerializedObject GetSerializedSettings() {
        return new SerializedObject(GetOrCreateSettings());
    }
}

static class UniWebViewSettingsProvider {
    static SerializedObject settings;

    #if UNITY_2018_3_OR_NEWER
    private class Provider : SettingsProvider {
        public Provider(string path, SettingsScope scope = SettingsScope.User): base(path, scope) {}
        public override void OnGUI(string searchContext) {
            DrawPref();
        }
    }
    [SettingsProvider]
    static SettingsProvider UniWebViewPref() {
        return new Provider("Preferences/UniWebView");
    }
    #else
    [PreferenceItem("UniWebView")]
    #endif
    static void DrawPref() {
        EditorGUIUtility.labelWidth = 320;
        if (settings == null) {
            settings = UniWebViewEditorSettings.GetSerializedSettings();
        }
        settings.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Android Manifest", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(settings.FindProperty("usesCleartextTraffic"));
        DrawDetailLabel("If you need to load plain HTTP content.");
        
        EditorGUILayout.PropertyField(settings.FindProperty("writeExternalStorage"));
        DrawDetailLabel("If you need to download an image from web page.");

        EditorGUILayout.PropertyField(settings.FindProperty("accessFineLocation"));
        DrawDetailLabel("If you need to enable location support in web view.");
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Gradle Build", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        EditorGUILayout.PropertyField(settings.FindProperty("addsKotlin"));
        DrawDetailLabel("Turn off this if another library is already adding Kotlin runtime.");
        EditorGUILayout.PropertyField(settings.FindProperty("addsAndroidBrowser"));
        DrawDetailLabel("Turn off this if another library is already adding 'androidx.browser:browser'.");

        EditorGUILayout.EndVertical();
        
        if (EditorGUI.EndChangeCheck()) {
            settings.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
        EditorGUIUtility.labelWidth = 0;
    }

    static void DrawDetailLabel(string text) {
        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField(text, EditorStyles.miniLabel);
        EditorGUI.indentLevel--;
    }
}