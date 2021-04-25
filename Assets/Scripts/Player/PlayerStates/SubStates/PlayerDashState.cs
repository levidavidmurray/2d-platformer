using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;
using Com.LuisPedroFonseca.ProCamera2D;

public class PlayerDashState : PlayerAbilityState
{

    public bool CanDash { get; private set; }

    private int xInput;
    private int yInput;

    private float lastDashTime;
    private Vector2 lastAfterImagePos;

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

        player.Trail.emitting = playerData.hasTrail;

        MasterAudio.PlaySoundAndForget("sfx_dash", playerData.dashSfxVolume);
        CanDash = false;
        lastDashTime = Time.time;
        player.InputHandler.UseDashInput();
        player.SetVelocityX(playerData.dashVelocity * player.FacingDirection);

        ProCamera2DShake.Instance.Shake(0);

        if (playerData.hasAfterImage) PlaceAfterImage();
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
        CheckIfShouldPlaceAfterImage();

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

    private void CheckIfShouldPlaceAfterImage()
    {
        if (!playerData.hasAfterImage) return;

        if (Vector2.Distance(player.transform.position, lastAfterImagePos) >= playerData.distBetweenAfterImages)
        {
            PlaceAfterImage();
        }
    }

    private void PlaceAfterImage()
    {
        var go = PlayerAfterImagePool.Instance.GetFromPool();
        PlayerAfterImageSprite ais = go.GetComponent<PlayerAfterImageSprite>();
        ais.alphaSet = playerData.afterImageAlphaSet;
        ais.alphaDecay = playerData.afterImageAlphaDecay;
        go.SetActive(false);
        go.SetActive(true);
        lastAfterImagePos = player.transform.position;
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
        player.Trail.emitting = false;
    }
}
