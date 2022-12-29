using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class HallUIView : UGUIView
    {
        public Button btnAgent;
        public Button btnShop;
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);
            Debug.Log("HallUIView OnInit ###");

            btnAgent.onClick.AddListener(() => {
                Debug.Log("btnAgent");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/AgentUIView.prefab");
            });
            btnShop.onClick.AddListener(() => {
                Debug.Log("btnShop");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/ShopUIView.prefab");
            });
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }

    }
}