using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class WordsRollUIView : UGUIView
    {
        public TextMeshProUGUI text;
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);
            text.transform.localPosition = new Vector3(600,0,0);
            text.transform.DOLocalMoveX(-600, 10);
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }

        public override void OnEnter(IUIContext uiConext, Action<string> callBack = null, params object[] parameters)
        {
            base.OnEnter(uiConext, callBack, parameters);
            //_canvasGroup.blocksRaycasts = false;
        }
    }
}
