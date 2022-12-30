using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class ServiceUIView : UGUIView
    {
        public Button btnClose;
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);

            btnClose.onClick.AddListener(() =>
            {
                GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/ServiceUIView.prefab"]);
            });
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }
    }
}
