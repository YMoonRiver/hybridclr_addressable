using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class SystemUIView : UGUIView
    {
        public Button btnClose;
        public Button btnYes;
        public Button btnCancel;
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);

            btnClose.onClick.AddListener(() =>
            {
                GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/SystemUIView.prefab"]);
            });
            btnYes.onClick.AddListener(() =>
            {
                GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/SystemUIView.prefab"]);
            });
            btnCancel.onClick.AddListener(() =>
            {
                GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/SystemUIView.prefab"]);
            });
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }
    }
}
