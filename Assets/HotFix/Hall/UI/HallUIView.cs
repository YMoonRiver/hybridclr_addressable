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
        public Button btnFirst;
        public Button btnRank;
        public Button btnAdd;
        public Button btnService;
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);
            Debug.Log("HallUIView OnInit ###");

            btnAgent.onClick.AddListener(() => {
                Debug.Log("btnAgent");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/AgentUIView.prefab");
            });
            btnShop.onClick.AddListener(() => {
                Debug.Log("btnShop");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/ShopUIView.prefab");
            });
            btnAdd.onClick.AddListener(() => {
                Debug.Log("btnAdd");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/ShopUIView.prefab");
            });
            btnFirst.onClick.AddListener(() => {
                Debug.Log("btnFirst");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/FirstUIView.prefab");
            });
            btnRank.onClick.AddListener(() => {
                Debug.Log("btnRank");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/RankUIView.prefab");
            });
            btnService.onClick.AddListener(() => {
                Debug.Log("btnService");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/ServiceUIView.prefab");
            });
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }

    }
}