using CircularScrollView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconList : MonoBehaviour
{
    public UICircularScrollView HorizontalScroll_2;
    // Start is called before the first frame update
    void Start()
    {
        HorizontalScroll_2.Init(NormalCallBack);
        HorizontalScroll_2.ShowList(500);
    }

    private void NormalCallBack(GameObject cell, int index) { }
}
