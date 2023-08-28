using DG.Tweening;
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
        public Button btnNotice;
        public Button btnWithdrawal;
        public Button btnActivity;
        public Button btnInfo;
        public Button btnDaily;
        public Button btnSetting;
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
            btnNotice.onClick.AddListener(() => {
                Debug.Log("btnNotice");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/NoticeUIView.prefab");
            });
            btnWithdrawal.onClick.AddListener(() => {
                Debug.Log("btnWithdrawal");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/WithdrawalUIView.prefab");
            });
            btnActivity.onClick.AddListener(() => {
                Debug.Log("btnActivity");
                var uiTween = GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/ActivityUIView.prefab");

                uiTween.NextUIView.transform.Find("Panel").localScale = Vector3.zero;
                //tween sample1
                var ds = DOTween.Sequence();
                ds.Append(uiTween.NextUIView.transform.Find("Panel").DOScale(1, 0.5f))
                .AppendCallback(()=> {
                    Log.Info("DOTween");
                });
                uiTween.SetAnimation(new UGUIAnimation().SetAnimation(ds));
                //tween sample2
                //IEnumerator Call2 = Call(uiTween);
                //uiTween.SetAnimation(new UGUIAnimation().SetAnimation(
                //    Call2
                //    ));

                uiTween.RunAnimation();
            });
            btnInfo.onClick.AddListener(() => {
                Debug.Log("btnInfo");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/InfoUIView.prefab");
            });
            btnDaily.onClick.AddListener(() => {
                Debug.Log("btnDaily");
                GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/DailyUIView.prefab");
            });
            btnSetting.onClick.AddListener(() => {
                Debug.Log("btnSetting");
                //GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/SettingUIView.prefab");
                GameMode.Event.Trigger(this, new StateEventArgs() { state = StateEventArgs.State.login });
            });
        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }


        IEnumerator Call(IUITween ut)
        {
            ut.NextUIView.transform.Find("Panel").localScale = Vector3.zero;
            yield return new WaitForEndOfFrame();
            ut.NextUIView.transform.Find("Panel").DOScale(1, 0.5f);
        }

    }
}