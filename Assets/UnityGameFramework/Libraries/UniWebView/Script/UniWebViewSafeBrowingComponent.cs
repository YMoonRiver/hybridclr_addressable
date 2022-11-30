using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniWebViewSafeBrowingComponent : MonoBehaviour
{
    [SerializeField]
    #pragma warning disable 0649
    private string url;

    void Start()
    {
        if (string.IsNullOrEmpty(url)) {
            Debug.LogError("The `url` is empty or null. Set a valid url in the prefab before you initialize it.");
            return;
        }
        var safeBrowsing = UniWebViewSafeBrowsing.Create(url);
        safeBrowsing.Show();
    }
}
