using DG.Tweening;
using System;
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
                GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/ShopUIView.prefab"]);
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

        public override void OnEnter(IUIContext uiConext, Action<string> callBack = null, params object[] parameters)
        {
            base.OnEnter(uiConext, callBack, parameters);
            transform.localScale = Vector3.zero;
            transform.DOScale(new Vector3(1, 1, 1), .5f);
        }
    }
}
