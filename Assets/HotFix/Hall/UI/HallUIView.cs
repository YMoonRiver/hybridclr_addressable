using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class HallUIView : UGUIView
    {
        Button btn;
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);
            Debug.Log("HallUIView OnInit ###");
            
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }

    }
}