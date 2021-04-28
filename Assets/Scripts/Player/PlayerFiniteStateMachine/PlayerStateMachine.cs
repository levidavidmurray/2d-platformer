using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }

    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        if (newState == null) return;

        if (CurrentState == null)
        {
            Initialize(newState);
            return;
        }

        UIManager.DebugUI.OnStateChange(newState);

        Debug.Log($"{CurrentState.GetType().Name} -> {newState.GetType().Name}");

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
