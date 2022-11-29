using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class LoadingEventArgs : GameEventArgs<LoadingEventArgs>
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        public string Tips;
        /// <summary>
        /// 场景加载进度
        /// </summary>
        public float Progress;
    }
}