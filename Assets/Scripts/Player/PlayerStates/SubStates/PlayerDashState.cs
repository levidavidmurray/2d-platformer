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

    private bool isTouchingWall = false;
    private int enterScaleTweenId;
    private int exitScaleTweenId;

    private float lastDashTime;
    private Vector2 lastAfterImagePos;

    private int lastPitchIndex;

    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingWall = player.CheckIfTouchingWall();
    }

    public override void Enter()
    {
        base.Enter();
        MasterAudio.StopAllOfSound("sfx_dash");

        CanDash = false;
        player.InputHandler.UseDashInput();
        lastDashTime = Time.time;

        player.Trail.emitting = playerData.hasTrail;

        float dashPitch = playerData.dashPitch;

        if (!playerData.useDashPitch)
        {
            int pitchIndex = lastPitchIndex;
            int attempts = 0;
            while (pitchIndex == lastPitchIndex && attempts < 3)
            {
                pitchIndex = Random.Range(0, playerData.dashPitchVariants.Length);
                attempts++;
            }
            lastPitchIndex = pitchIndex;
            dashPitch = playerData.dashPitchVariants[pitchIndex];
        }

        LeanTween.cancel(exitScaleTweenId);

        MasterAudio.PlaySoundAndForget("sfx_dash", playerData.trackSfxVolume);

        player.TrackEffects.Enable();
        enterScaleTweenId = player.gameObject.LeanScale(playerData.dashScale * Vector3.one, playerData.dashScaleTime).id;

        ProCamera2DShake.Instance.Shake("GunShot");

        player.CheckIfShouldFlip(player.InputHandler.NormInputX);

        player.RBFreezeY();
        player.SetVelocityX(playerData.dashVelocity * player.FacingDirection);

        if (playerData.hasAfterImage) PlaceAfterImage();
    }

    public override void Exit()
    {
        base.Exit();

        LeanTween.cancel(enterScaleTweenId);

        player.Trail.emitting = false;
        player.RBResume();
        exitScaleTweenId = LeanTween.delayedCall(playerData.dashExitScaleDelay, () => {
            exitScaleTweenId = player.gameObject.LeanScale(Vector3.one, playerData.dashScaleTime).id;
            player.TrackEffects.Disable();
        }).id;
        // MasterAudio.FadeOutAllOfSound("sfx_current", playerData.trackSfxFadeTime);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;

        CheckIfShouldPlaceAfterImage();

        if (isTouchingWall)
        {
            this.EndDash();
        }

        if (Time.time - lastDashTime >= playerData.dashTime)
        {
            this.EndDash();
        }
        else if (Time.time - lastDashTime >= playerData.dashTime / 2)
        {
            // MasterAudio.FadeOutAllOfSound("sfx_current", playerData.trackSfxFadeTime);
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
        LeanTween.cancel(enterScaleTweenId);
        player.SetVelocityX(0);
        isAbilityDone = true;
        player.Trail.emitting = false;
        player.RBResume();
    }
}
