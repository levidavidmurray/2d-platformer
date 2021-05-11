using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;
using Com.LuisPedroFonseca.ProCamera2D;

public enum DashDirection {
    NORTH,
    EAST,
    SOUTH,
    WEST,
    NORTH_EAST,
    SOUTH_EAST,
    SOUTH_WEST,
    NORTH_WEST,
}

public class PlayerDashState : PlayerAbilityState
{

    public DashDirection dashDirection { get; private set; }
    public Vector2 dashInputDirection;

    private int xInput;
    private int yInput;
    private bool dashInput;
    private bool dashInputStop;

    private bool isTouchingWall = false;
    private int enterScaleTweenId;
    private int exitScaleTweenId;

    private int numOfDashesLeft;

    private float dashTime;
    private float lastDashTime;
    private Vector2 lastAfterImagePos;

    private int lastPitchIndex;

    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
        numOfDashesLeft = playerData.numOfDashes;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingWall = player.CheckIfTouchingWall(dashInputDirection);
    }

    public override void Enter()
    {
        base.Enter();
        MasterAudio.StopAllOfSound("sfx_dash");

        numOfDashesLeft--;
        player.InputHandler.UseDashInput();
        lastDashTime = Time.time;

        player.Trail.emitting = playerData.hasTrail;

        dashInputDirection = player.InputHandler.SnappedMovementInput;

        dashDirection = FindDashDirection(dashInputDirection);

        LeanTween.cancel(exitScaleTweenId);

        MasterAudio.PlaySoundAndForget("sfx_dash", playerData.dashSfxVolume, FindDashPitch(dashDirection));

        player.TrackEffects.Enable();
        enterScaleTweenId = player.gameObject.LeanScale(playerData.dashScale * Vector3.one, playerData.dashScaleTime).id;

        ProCamera2DShake.Instance.Shake("GunShot");

        player.CheckIfShouldFlip(player.InputHandler.NormInputX);

        if (dashInputDirection == Vector2.zero)
        {
            dashInputDirection = Vector2.right * player.FacingDirection;
        }

        float dashVelocity = playerData.dashVelocity.x;
        dashTime = playerData.dashTime.x;

        if (dashDirection == DashDirection.EAST || dashDirection == DashDirection.WEST)
        {
            player.RBFreezeY();
        }
        else if (dashDirection == DashDirection.NORTH || dashDirection == DashDirection.SOUTH)
        {
            dashVelocity = playerData.dashVelocity.y;
            dashTime = playerData.dashTime.y;
        }
        else
        {
            // Diagonal direction
            dashVelocity = (playerData.dashVelocity.x + playerData.dashVelocity.y) / 2;
            dashTime = (playerData.dashTime.x + playerData.dashTime.y) / 2;
        }

        player.SetVelocity(dashVelocity, dashInputDirection, 1);
        // player.SetVelocityX(playerData.dashVelocity * player.FacingDirection);

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
        dashInput = player.InputHandler.DashInput;
        dashInputStop = player.InputHandler.DashInputStop;

        CheckIfShouldPlaceAfterImage();

        if (dashInputStop || isTouchingWall)
        {
            this.EndDash();
        }
        else if (Time.time - lastDashTime >= dashTime)
        {
            this.EndDash();
        }
        else if (xInput != 0 && xInput != player.FacingDirection)
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

    public bool CanDash {
        get {
            return Time.time - lastDashTime >= playerData.dashCooldown && numOfDashesLeft > 0;
        }
    }

    public void ResetCanDash() => numOfDashesLeft = playerData.numOfDashes;

    private void EndDash()
    {
        LeanTween.cancel(enterScaleTweenId);
        player.FreezeVelocity();
        isAbilityDone = true;
        player.Trail.emitting = false;
        player.RBResume();
    }

    private DashDirection FindDashDirection(Vector2 snappedDirectionInput)
    {

        if (snappedDirectionInput == Vector2.up)
            return DashDirection.NORTH;

        if (snappedDirectionInput == Vector2.right)
            return DashDirection.EAST;

        if (snappedDirectionInput == Vector2.down)
            return DashDirection.SOUTH;

        if (snappedDirectionInput == Vector2.left)
            return DashDirection.WEST;

        if (snappedDirectionInput.x > 0 && snappedDirectionInput.y > 0)
            return DashDirection.NORTH_EAST;

        if (snappedDirectionInput.x > 0 && snappedDirectionInput.y < 0)
            return DashDirection.SOUTH_EAST;

        if (snappedDirectionInput.x < 0 && snappedDirectionInput.y < 0)
            return DashDirection.SOUTH_WEST;

        if (snappedDirectionInput.x < 0 && snappedDirectionInput.y > 0)
            return DashDirection.NORTH_WEST;

        // Fallback -- In theory shouldn't happen
        return DashDirection.EAST;
    }

    public float FindDashPitch(DashDirection direction)
    {
        switch(direction)
        {
            case DashDirection.NORTH:
                return playerData.dashNorthPitch;
            case DashDirection.EAST:
                return playerData.dashEastPitch;
            case DashDirection.SOUTH:
                return playerData.dashSouthPitch;
            case DashDirection.WEST:
                return playerData.dashWestPitch;
            case DashDirection.NORTH_EAST:
                return playerData.dashNorthEastPitch;
            case DashDirection.SOUTH_EAST:
                return playerData.dashSouthEastPitch;
            case DashDirection.SOUTH_WEST:
                return playerData.dashSouthWestPitch;
            case DashDirection.NORTH_WEST:
                return playerData.dashNorthWestPitch;
            default:
                // Fallback -- In theory shouldn't happen
                return playerData.dashEastPitch;
        }
    }
}
