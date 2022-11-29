using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wanderer.GameFramework;

public class Loading : MonoBehaviour
{
    public Slider sliderLoading;
    public Text txtTips;
    // Start is called before the first frame update
    void Start()
    {
        GameFrameworkMode.GetModule<EventManager>().AddListener<LoadingEventArgs>(OnLoading);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnLoading(object sender, IEventArgs e)
    {
        var args = (LoadingEventArgs)e;
        sliderLoading.value = args.Progress;
        txtTips.text = args.Tips;
    }
}
