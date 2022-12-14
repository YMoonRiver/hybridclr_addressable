using HybridCLR;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Wanderer.GameFramework;

public class HotUpdateMain : MonoBehaviour
{

    public string text;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("这个热更新脚本挂载在prefab上，打包成ab。通过从ab中实例化prefab成功还原");
        Debug.LogFormat("hello, {0}.", text);

        gameObject.AddComponent<CreateByCode>();

        Debug.Log("=======看到此条日志代表你成功运行了示例项目的热更新代码=======");

        //开始游戏
        Debug.Log("PlayStateContext Begin");
        GameMode.FSM.AddFSM<PlayStateContext>();
        FSMState<PlayStateContext>[] states = {
                new GameState(),
                new HallState(),
                new LoginState(),
                new SelectState(),
            };
        GameMode.FSM.GetFSM<PlayStateContext>().Creat(states);
        GameMode.FSM.GetFSM<PlayStateContext>().OnBegin();

        //Observable.Timer(System.TimeSpan.FromSeconds(2))
        //    .Subscribe(_ => { Debug.Log("delay 2 seconds"); });

        //Observable.EveryUpdate()
        //        .Where(_ => Input.GetMouseButtonDown(0))
        //        .Subscribe(_ =>
        //        {

        //            Debug.Log("鼠标点击");
        //        });

        //Observable.EveryUpdate().
        //        Where(_ => Input.GetMouseButtonDown(0)) //相当于if 判断
        //        .First() //只执行一次  Subscribe：订阅的事件或者回调
        //        .Subscribe(_ =>
        //        {
        //            Debug.Log("鼠标只能点击一次");

        //        }).AddTo(this);

        //Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0))
        //            .Throttle(System.TimeSpan.FromSeconds(1))
        //            .Subscribe(_ => Debug.Log("一秒过后"));

     
    }

}
