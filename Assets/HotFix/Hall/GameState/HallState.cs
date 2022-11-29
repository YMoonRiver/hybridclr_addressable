using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wanderer.GameFramework;

[FSM()]
public class HallState : FSMState<PlayStateContext>
{
    #region ÖØÐ´º¯Êý
    public override void OnEnter(FSM<PlayStateContext> fsm)
    {
        base.OnEnter(fsm);
        Debug.Log("HallState");
        GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/HallUIView.prefab");
        GameMode.Resource.Asset.LoadSceneAsync("Assets/Addressable/Hall/Scenes/Hall.unity", UnityEngine.SceneManagement.LoadSceneMode.Single,
            (obj) =>
            {
                //ChangeState<HallState>(fsm);
               
                Debug.Log("Login.unity");
            });
    }

    public override void OnExit(FSM<PlayStateContext> fsm)
    {
        base.OnExit(fsm);
    }

    public override void OnInit(FSM<PlayStateContext> fsm)
    {
        base.OnInit(fsm);
    }

    public override void OnUpdate(FSM<PlayStateContext> fsm)
    {
        base.OnUpdate(fsm);

    }
    #endregion
}
