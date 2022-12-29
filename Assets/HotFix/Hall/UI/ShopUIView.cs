using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class ShopUIView : UGUIView
    {
        public Button btnClose;
        public Toggle[] toggles;
        public GameObject[] objs;
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);

            btnClose.onClick.AddListener(() => {
                GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/ShopUIView.prefab"]);
            });


            toggles[0].onValueChanged.AddListener((b) => {
                objs[0].SetActive(b);
            });
            toggles[1].onValueChanged.AddListener((b) => {
                objs[1].SetActive(b);
            });
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }

    }
}
