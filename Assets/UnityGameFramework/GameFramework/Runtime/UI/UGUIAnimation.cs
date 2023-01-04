using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class UGUIAnimation : IUIAnimation
    {
        public UIView TargetUIView { get; set; }

        private IEnumerator _enumerator;
        private Tween _dotweenTweener;

        public void OnUITweenComplete()
        {
            Flush();
        }

        public async UniTask<int> Run()
        {
            if (_dotweenTweener != null)
            {
                await _dotweenTweener;
            }
            if (_enumerator != null)
            {
                await _enumerator;
            }
            return 1;
        }

        public void SetTarget(GameObject target)
        {
            TargetUIView.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void Flush()
        {
            _dotweenTweener = null;
            _enumerator = null;
            TargetUIView = null;
        }

        public IUIAnimation SetAnimation(IEnumerator enumerator)
        {
            _enumerator = enumerator;
            return this;
        }

        public IUIAnimation SetAnimation(Tween tweener)
        {
            _dotweenTweener = tweener;
            return this;
        }
    }
}