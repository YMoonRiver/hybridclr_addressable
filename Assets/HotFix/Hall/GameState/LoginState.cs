using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wanderer.GameFramework;

[FSM(FSMStateType.Start)]
public class LoginState : FSMState<PlayStateContext>
{
    FSM<PlayStateContext> _fsm;

    #region ÖØÐ´º¯Êý
    public override void OnInit(FSM<PlayStateContext> fsm)
    {
        base.OnInit(fsm);
        _fsm = fsm;
    }
    public override void OnEnter(FSM<PlayStateContext> fsm)
    {
        base.OnEnter(fsm);
        Debug.Log("LoginState");
        GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/LoginUIView.prefab");
        GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/SystemUIView.prefab");
        GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/WordsRollUIView.prefab");

        GameMode.Resource.Asset.LoadSceneAsync("Assets/Addressable/Hall/Scenes/Login.unity", UnityEngine.SceneManagement.LoadSceneMode.Single,
            (obj) =>
            {
                Debug.Log("Login.unity");
            });

        GameFrameworkMode.GetModule<EventManager>().AddListener<StateEventArgs>(OnLogin);
    }

    public override void OnExit(FSM<PlayStateContext> fsm)
    {
        base.OnExit(fsm);
        GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/LoginUIView.prefab"]);
        GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/WordsRollUIView.prefab"],true,true);
        GameFrameworkMode.GetModule<EventManager>().RemoveListener<StateEventArgs>(OnLogin);

        GameMode.Resource.Asset.UnloadSceneAsync("Assets/Addressable/Hall/Scenes/Login.unity");
    }

    public override void OnUpdate(FSM<PlayStateContext> fsm)
    {
        base.OnUpdate(fsm);
    }
    #endregion

    private void OnLogin(object sender, IEventArgs e) 
    {
        Debug.Log("OnHall");
        var args = (StateEventArgs)e;
        if (args.state == StateEventArgs.State.hall)
        {
            ChangeState<HallState>(_fsm);
        }
    }
}