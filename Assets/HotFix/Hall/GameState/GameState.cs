using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wanderer.GameFramework;

[FSM()]
public class GameState : FSMState<PlayStateContext>
{
    #region ÖØÐ´º¯Êý
    public override void OnEnter(FSM<PlayStateContext> fsm)
    {
        base.OnEnter(fsm);
        Debug.Log("GameState");

        //ChangeState<HallState>(fsm);
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
