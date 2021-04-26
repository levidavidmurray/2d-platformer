using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected bool isAnimationFinished;
    protected bool isExitingState;

    protected float startTime;
    private string animBoolName;

    public PlayerState(
        Player player, 
        PlayerStateMachine stateMachine, 
        PlayerData playerData, 
        string animBoolName
    )
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
    }

    public virtual void Die()
    {
        stateMachine.ChangeState(player.DeadState);
    }

    // Enter State
    public virtual void Enter()
    {
        DoChecks();
        player.Anim.SetBool(animBoolName, true);
        startTime = Time.time;
        isAnimationFinished = false;
        isExitingState = false;
    }

    // Exit State
    public virtual void Exit()
    {
        player.Anim.SetBool(animBoolName, false);
        isExitingState = true;
    }

    // Called Every Frame
    public virtual void LogicUpdate()
    {

    }

    // Called Every Physics Update
    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    // Called from Enter & PhysicsUpdate (Check for ground, walls, etc.)
    public virtual void DoChecks()
    {

    }

    public virtual void AnimationTrigger()
    {

    }

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;

}
