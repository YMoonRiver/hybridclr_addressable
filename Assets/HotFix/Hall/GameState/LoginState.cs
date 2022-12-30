using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wanderer.GameFramework;

[FSM(FSMStateType.Start)]
public class LoginState : FSMState<PlayStateContext>
{
    private bool _flag = false;
    private bool _flagLogin = false;
    #region ÖØÐ´º¯Êý
    public override void OnInit(FSM<PlayStateContext> fsm)
    {
        base.OnInit(fsm);
    }
    public override void OnEnter(FSM<PlayStateContext> fsm)
    {
        base.OnEnter(fsm);
        _flag = false;

        GameMode.Resource.Asset.LoadSceneAsync("Assets/Addressable/Hall/Scenes/Login.unity", UnityEngine.SceneManagement.LoadSceneMode.Single,
            (obj) =>
            {
                //ChangeState<HallState>(fsm);
                Debug.Log("Login.unity");
                _flagLogin = true;
            });

        GameFrameworkMode.GetModule<EventManager>().AddListener<LoginToHallEventArgs>(OnHall);
    }

    public override void OnExit(FSM<PlayStateContext> fsm)
    {
        base.OnExit(fsm);
        GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/LoginUIView.prefab"]);
        GameFrameworkMode.GetModule<EventManager>().RemoveListener<LoginToHallEventArgs>(OnHall);
    }

    public override void OnUpdate(FSM<PlayStateContext> fsm)
    {
        base.OnUpdate(fsm);

        if (_flag)
        {
            ChangeState<HallState>(fsm);
        }

        if (_flagLogin)
        {
            _flagLogin = false;
            GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/LoginUIView.prefab");
            GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/SystemUIView.prefab");
            GameMode.UI.Push("Assets/Addressable/Hall/Prefabs/UI/WordsRollUIView.prefab");
            GameMode.UI.Close(GameMode.UI.UIContextMgr["Assets/Addressable/Hall/Prefabs/UI/WordsRollUIView.prefab"]);
        }
    }
    #endregion

    private void OnHall(object sender, IEventArgs e) 
    {
        Debug.Log("OnHall");
        _flag = true;
    }
}