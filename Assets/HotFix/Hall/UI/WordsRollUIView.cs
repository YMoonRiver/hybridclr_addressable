using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class WordsRollUIView : UGUIView
    {
        public TextMeshProUGUI text;
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);
            
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
            
        }

        public override void OnEnter(IUIContext uiConext, Action<string> callBack = null, params object[] parameters)
        {
            base.OnEnter(uiConext, callBack, parameters);
            text.transform.localPosition = new Vector3(600, 0, 0);
            text.transform.DOLocalMoveX(-600, 10);

            Observable.Timer(System.TimeSpan.FromSeconds(10))
                .Subscribe(_ => { 
                    Debug.Log("delay 10 seconds");
                    GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/WordsRollUIView.prefab"], true, true);
                }).AddTo(this);
        }

        public override void OnExit(IUIContext uiConext)
        {
            base.OnExit(uiConext);
        }
    }
}
