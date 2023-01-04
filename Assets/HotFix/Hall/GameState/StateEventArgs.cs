using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wanderer.GameFramework;

public class StateEventArgs : GameEventArgs<StateEventArgs> {
    public enum State {
        login = 1,
        hall = 2,
        select = 3,
        game = 4,
    };

    public State state; 
}