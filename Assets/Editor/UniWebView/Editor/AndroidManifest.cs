using System.Xml;
using System.Collections;
using System.Text;
using System.IO;

internal class UniWebViewAndroidXmlDocument : XmlDocument {
    private string path;
    protected XmlNamespaceManager nameSpaceManager;
    public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

    public UniWebViewAndroidXmlDocument(string path) {
        this.path = path;
        using (var reader = new XmlTextReader(path)) {
            reader.Read();
            Load(reader);
        }
        nameSpaceManager = new XmlNamespaceManager(NameTable);
        nameSpaceManager.AddNamespace("android", AndroidXmlNamespace);
    }

    public string Save() {
        return SaveAs(path);
    }

    public string SaveAs(string path) {
        using (var writer = new XmlTextWriter(path, new UTF8Encoding(false))) {
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }
        return path;
    }
}

internal class UniWebViewAndroidManifest : UniWebViewAndroidXmlDocument {
    private readonly XmlElement ManifestElement;
    private readonly XmlElement ApplicationElement;

    public UniWebViewAndroidManifest(string path) : base(path) {
        ManifestElement = SelectSingleNode("/manifest") as XmlElement;
        ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
    }

    private XmlAttribute CreateAndroidAttribute(string key, string value) {
        XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
        attr.Value = value;
        return attr;
    }

    internal XmlNode GetActivityWithLaunchIntent() {
        return
            SelectSingleNode(
                "/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and "
                + "intent-filter/category/@android:name='android.intent.category.LAUNCHER']",
                nameSpaceManager);
    }

    internal bool SetUsesCleartextTraffic() {
        bool changed = false;
        if (ApplicationElement.GetAttribute("usesCleartextTraffic", AndroidXmlNamespace) != "true") {
            ApplicationElement.SetAttribute("usesCleartextTraffic", AndroidXmlNamespace, "true");
            changed = true;
        }
        return changed;
    }

    internal bool SetHardwareAccelerated() {
        bool changed = false;
        var activity = GetActivityWithLaunchIntent() as XmlElement;
        if (activity.GetAttribute("hardwareAccelerated", AndroidXmlNamespace) != "true") {
            activity.SetAttribute("hardwareAccelerated", AndroidXmlNamespace, "true");
            changed = true;
        }
        return changed;
    }

    internal bool AddCameraPermission() {
        bool changed = false;
        if (SelectNodes("/manifest/uses-permission[@android:name='android.permission.CAMERA']", nameSpaceManager).Count == 0) {
            var elem = CreateElement("uses-permission");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.CAMERA"));
            ManifestElement.AppendChild(elem);
            changed = true;
        }
        if (SelectNodes("/manifest/uses-feature[@android:name='android.hardware.camera']", nameSpaceManager).Count == 0) {
            var elem = CreateElement("uses-feature");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.hardware.camera"));
            ManifestElement.AppendChild(elem);
            changed = true;
        }
        return changed;
    }

    internal bool AddMicrophonePermission() {
        bool changed = false;
        if (SelectNodes("/manifest/uses-permission[@android:name='android.permission.MICROPHONE']", nameSpaceManager).Count == 0) {
            var elem = CreateElement("uses-permission");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.MICROPHONE"));
            ManifestElement.AppendChild(elem);
            changed = true;
        }
        if (SelectNodes("/manifest/uses-feature[@android:name='android.hardware.microphone']", nameSpaceManager).Count == 0) {
            var elem = CreateElement("uses-feature");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.hardware.microphone"));
            ManifestElement.AppendChild(elem);
            changed = true;
        }
        return changed;
    }

    internal bool AddReadExternalStoragePermission() {
        bool changed = false;
        if (SelectNodes("/manifest/uses-permission[@android:name='android.permission.READ_EXTERNAL_STORAGE']", nameSpaceManager).Count == 0) {
            var elem = CreateElement("uses-permission");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.READ_EXTERNAL_STORAGE"));
            ManifestElement.AppendChild(elem);
            changed = true;
        }
        return changed;
    }

    internal bool AddWriteExternalStoragePermission() {
        bool changed = false;
        if (SelectNodes("/manifest/uses-permission[@android:name='android.permission.WRITE_EXTERNAL_STORAGE']", nameSpaceManager).Count == 0) {
            var elem = CreateElement("uses-permission");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.WRITE_EXTERNAL_STORAGE"));
            ManifestElement.AppendChild(elem);
            changed = true;
        }
        return changed;
    }

    internal bool AddAccessFineLocationPermission() {
        bool changed = false;
        if (SelectNodes("/manifest/uses-permission[@android:name='android.permission.ACCESS_FINE_LOCATION']", nameSpaceManager).Count == 0) {
            var elem = CreateElement("uses-permission");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.ACCESS_FINE_LOCATION"));
            ManifestElement.AppendChild(elem);
            changed = true;
        }
        return changed;
    }
}