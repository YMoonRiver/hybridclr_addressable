//
//  UniWebViewSafeBrowsing.cs
//  Created by Wang Wei(@onevcat) on 2020-07-18.
//
//  This file is a part of UniWebView Project (https://uniwebview.com)
//  By purchasing the asset, you are allowed to use this code in as many as projects 
//  you want, only if you publish the final products under the name of the same account
//  used for the purchase. 
//
//  This asset and all corresponding files (such as source code) are provided on an 
//  “as is” basis, without warranty of any kind, express of implied, including but not 
//  limited to the warranties of merchantability, fitness for a particular purpose, and 
//  noninfringement. In no event shall the authors or copyright holders be liable for any 
//  claim, damages or other liability, whether in action of contract, tort or otherwise, 
//  arising from, out of or in connection with the software or the use of other dealing in the software.
//

using UnityEngine;
using System;

/// <summary>
/// UniWebView Safe Browsing provides a way for browsing the web content in a more browser-like way, such as Safari on 
/// iOS and Chrome on Android.
/// 
/// This class wraps `SFSafariViewController` on iOS and "Custom Tabs" on Android. It shares cookies, auto-fill 
/// completion and other more data with the browser on device. Most of permissions are also built-in supported. You can
/// use this class for some tasks that are limited for a normal web view, such as using Apple Pay or Progressive Web 
/// Apps (PWA).
/// 
/// You create a `UniWebViewSafeBrowsing` instance by calling the static `UniWebViewSafeBrowsing.Create` method with a
/// destination URL. You cannot change this URL once the instance is created. To show the safe browsing, call `Show` on
/// the instance. The web content will be displayed in full screen with a toolbar containing the loaded URL, as well
/// as some basic controls like Go Back, Go Forward and Done. 
/// 
/// Browsing web content in `UniWebViewSafeBrowsing` is only supported on iOS and Android. There is no such component in
/// Unity Editor. Creating and showing a `UniWebViewSafeBrowsing` on Unity Editor will fall back to open the URL in 
/// external browser by using Unity's `Application.OpenURL`.
/// 
/// </summary>
public class UniWebViewSafeBrowsing: UnityEngine.Object {

    /// <summary>
    /// Delegate for safe browsing finish event.
    /// </summary>
    /// <param name="browsing">The `UniWebViewSafeBrowsing` object raised this event.</param>
    public delegate void OnSafeBrowsingFinishedDelegate(UniWebViewSafeBrowsing browsing);
    /// <summary>
    /// Raised when user dismisses safe browsing by tapping the Done button or Back button.
    /// 
    /// The dismissed safe browsing instance will be invalid after this event being raised, and you should not use 
    /// it for another browsing purpose. Instead, create a new one for a new browsing session.
    /// 
    /// This event will not happen in Unity Editor, because the whole `UniWebViewSafeBrowsing` will fall back to an 
    /// external browser.
    /// </summary>
    public event OnSafeBrowsingFinishedDelegate OnSafeBrowsingFinished;
    
    private string id = Guid.NewGuid().ToString();
    private UniWebViewNativeListener listener;

    // This is only for editor, to open the url in system browser.
    private string url;

    /// <summary>
    /// Whether the safe browsing mode is supported in current runtime or not.
    /// 
    /// If supported, the safe browsing mode will be used when `Show` is called on a `UniWebViewSafeBrowsing` instance.
    /// Otherwise, the system default browser will be used to open the page when `Show` is called.
    /// 
    /// This property always returns `true` on iOS runtime platform. On Android, it depends on whether the Chrome app, 
    /// which underhood provides the ability of safe browsing, is installed or not.
    /// </summary>
    /// <value>
    /// Returns `true` if the safe browsing mode is supported and the page will be opened in safe browsing 
    /// mode. Otherwise, `false`.
    /// </value>
    public static bool IsSafeBrowsingSupported {
        get {
            #if UNITY_EDITOR
            return false;
            #elif UNITY_IOS
            return true;
            #elif UNITY_ANDROID
            return UniWebViewInterface.IsSafeBrowsingSupported();
            #else
            return false; 
            #endif
        }
    }

    /// <summary>
    /// Creates a new `UniWebViewSafeBrowsing` instance with a given URL.
    /// </summary>
    /// <param name="url">The URL to navigate to. The URL must use the `http` or `https` scheme.</param>
    /// <returns>A newly created `UniWebViewSafeBrowsing` instance.</returns>
    public static UniWebViewSafeBrowsing Create(string url) {
        var safeBrowsing = new UniWebViewSafeBrowsing();
        if (!UniWebViewHelper.IsEditor) {
            safeBrowsing.listener.safeBrowsing = safeBrowsing;
            safeBrowsing.Init(url);
        }
        safeBrowsing.url = url;
        
        return safeBrowsing;
    }

    /// <summary>
    /// Shows the safe browsing content above current screen.
    /// </summary>
    public void Show() {
        if (UniWebViewHelper.IsEditor) {
            Application.OpenURL(url);
        } else {
            UniWebViewInterface.SafeBrowsingShow(listener.Name);
        }
    }

    /// <summary>
    /// Dismisses the safe browsing component.
    /// 
    /// This method only works on iOS. On Android, there is no way to dismiss the safe browsing component 
    /// programatically as the result of the limitation from the native (Android) side.
    /// </summary>
    public void Dismiss() {
        #if UNITY_IOS && !UNITY_EDITOR
        UniWebViewInterface.SafeBrowsingDismiss(listener.Name);
        #endif
    }

    /// <summary>
    /// Sets the color for toolbar background in the safe browsing component. The changes are ignored after `Show`
    /// method is called.
    /// </summary>
    /// <param name="color">The color to tint the toolbar.</param>
    public void SetToolbarColor(Color color) {
        if (!UniWebViewHelper.IsEditor) {
            UniWebViewInterface.SafeBrowsingSetToolbarColor(listener.Name, color.r, color.g, color.b);
        }
    }

    /// <summary>
    /// Sets the color for toolbar controls in the safe browsing component. The changes are ignored after `Show` method
    /// is called.
    /// 
    /// This method only works on iOS. On Android, the controls color is determined by system to keep a reasonable 
    /// contrast, based on the toolbar background color you provided in `SetToolbarColor`.
    /// </summary>
    /// <param name="color">The color to tint the controls on toolbar.</param>
    public void SetToolbarItemColor(Color color) {
        #if UNITY_IOS && !UNITY_EDITOR
        UniWebViewInterface.SafeBrowsingSetToolbarItemColor(listener.Name, color.r, color.g, color.b);
        #endif
    }

    private UniWebViewSafeBrowsing() {
        if (!UniWebViewHelper.IsEditor) {
            var listenerObject = new GameObject(id);
            listener = listenerObject.AddComponent<UniWebViewNativeListener>();
            UniWebViewNativeListener.AddListener(listener);
        }
    }

    private void Init(string url) {
        UniWebViewInterface.SafeBrowsingInit(listener.Name, url);
    }

    internal void InternalSafeBrowsingFinished() {
        if (OnSafeBrowsingFinished != null) {
            OnSafeBrowsingFinished(this);
        }

        UniWebViewNativeListener.RemoveListener(listener.Name);
        Destroy(listener.gameObject);
    }
}