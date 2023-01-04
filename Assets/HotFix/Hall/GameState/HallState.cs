using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wanderer.GameFramework;

[FSM()]
public class HallState : FSMState<PlayStateContext>
{
    FSM<PlayStateContext> _fsm;

    #region ÖØÐ´º¯Êý
    public override void OnEnter(FSM<PlayStateContext> fsm)
    {
        base.OnEnter(fsm);
        Debug.Log("HallState");
        GameFrameworkMode.GetModule<EventManager>().AddListener<StateEventArgs>(OnHall);

        GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/HallUIView.prefab");
        
        GameMode.Resource.Asset.LoadSceneAsync("Assets/Addressable/Hall/Scenes/Hall.unity", UnityEngine.SceneManagement.LoadSceneMode.Single,
            (obj) =>
            {
                Debug.Log("Hall.unity");
            });
    }

    public override void OnExit(FSM<PlayStateContext> fsm)
    {
        base.OnExit(fsm);
        GameFrameworkMode.GetModule<EventManager>().RemoveListener<StateEventArgs>(OnHall);
        GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/HallUIView.prefab"]);
        GameMode.Resource.Asset.UnloadSceneAsync("Assets/Addressable/Hall/Scenes/Hall.unity");
    }

    public override void OnInit(FSM<PlayStateContext> fsm)
    {
        base.OnInit(fsm);
        _fsm = fsm;
    }

    public override void OnUpdate(FSM<PlayStateContext> fsm)
    {
        base.OnUpdate(fsm);
    }
    #endregion

    private void OnHall(object arg1, IEventArgs e)
    {
        Debug.Log("OnHall");
        var args = (StateEventArgs)e;
        if (args.state == StateEventArgs.State.login)
        {
            ChangeState<LoginState>(_fsm);
        }
    }
}
