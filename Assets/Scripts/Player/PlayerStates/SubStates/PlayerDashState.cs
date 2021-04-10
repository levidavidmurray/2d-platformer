using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{

    public bool CanDash { get; private set; }

    private int xInput;
    private int yInput;

    private float lastDashTime;

    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        CanDash = false;
        lastDashTime = Time.time;
        player.InputHandler.UseDashInput();
        player.SetVelocityX(playerData.dashVelocity * player.FacingDirection);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;

        player.SetVelocityY(0f);

        if (Time.time - lastDashTime >= playerData.dashTime)
        {
            this.EndDash();
        }
        else if (xInput != 0 && xInput != player.FacingDirection)
        {
            this.EndDash();
        }
        else if (yInput == -1)
        {
            this.EndDash();
        }
    }

    public void CheckDashCooldown()
    {
        if (Time.time - lastDashTime >= playerData.dashCooldown)
        {
            CanDash = true;
        }
    }

    public void ResetCanDash() => CanDash = true;

    private void EndDash()
    {
        player.SetVelocityX(0);
        isAbilityDone = true;
    }
}
