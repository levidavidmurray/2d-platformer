using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

public class PlayerTrackState : PlayerAbilityState
{

    private int currentTrack;
    private TrackPoint trackPoint;
    private TrackPoint targetTrackPoint;

    private Vector2 startPosition;
    private Vector2 endPosition;
    private Vector2 direction;

    private float timeOnPoint = 0f;
    private float timeToPoint = 0f;
    private float trackExitTime = 0f;

    private bool sfxFadeStarted = false;
    private bool isMovingToEnterPoint = true;
    private int lastTrackHashCode;

    public PlayerTrackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.DashState.ResetCanDash();
        player.JumpState.ResetAmountOfJumpsLeft();
        player.TrackEffects.Enable();

        MasterAudio.StopAllOfSound("sfx_electric_current");
        MasterAudio.StopAllOfSound("sfx_electric_current_end");

        MasterAudio.PlaySound(
            "sfx_electric_current", 
            playerData.trackSfxLoopVolume, 
            null, 
            playerData.trackSfxLoopDelay
        );

        LeanTween.scale(player.gameObject, playerData.trackPlayerScale, playerData.trackPlayerEnterScaleTime);

        player.RB.simulated = false;
        player.FreezeVelocity();

        CalculatePoints(player.transform.position, trackPoint.Position);

        isMovingToEnterPoint = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void CalculatePoints(Vector2 startPos, Vector2 endPos)
    {

        startPosition = startPos;
        endPosition = endPos;
        timeOnPoint = 0f;

        var heading = endPos - startPos;
        var dist = Vector3.Distance(startPos,endPos);
        direction = heading / dist;

        timeToPoint = dist / playerData.trackVelocity;
        Debug.Log($"CalculatePoints startPos: {startPos}, endPos: {endPos}, dist: {dist}, timeToPoint: {timeToPoint}");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAbilityDone) return;


        timeOnPoint += Time.deltaTime;

        var pointPathPercent = timeOnPoint / timeToPoint;

        // Fade out SFX at specific percent of track complete
        if (
            !targetTrackPoint.HasTrackTarget(currentTrack) && 
            !isMovingToEnterPoint &&
            timeToPoint - timeOnPoint <= playerData.trackSfxFadeExitTime && 
            !sfxFadeStarted)
        {
            Debug.Log($"FADE sfx_current: timeToPoint: {timeToPoint}, timeOnPoint: {timeOnPoint}");
            MasterAudio.FadeOutAllOfSound("sfx_electric_current", playerData.trackSfxFadeTime);
            MasterAudio.PlaySoundAndForget("sfx_electric_current_end", playerData.trackSfxEndVolume);
            sfxFadeStarted = true;
        }

        player.transform.position = Vector3.Lerp(startPosition, endPosition, pointPathPercent);

        if (pointPathPercent >= 0.98f)
        {
            if (isMovingToEnterPoint)
            {
                isMovingToEnterPoint = false;
                CalculatePoints(trackPoint.Position, targetTrackPoint.Position);
            }
            else
            {
                trackPoint = targetTrackPoint;
                targetTrackPoint = trackPoint.GetTarget(currentTrack);
                if (targetTrackPoint == null)
                {
                    ExitTrack();
                    return;
                }
                else
                {
                    CalculatePoints(trackPoint.Position, targetTrackPoint.Position);
                }
            }
        }

    }

    private void ExitTrack()
    {
        player.RB.simulated = true;
        player.SetVelocity(playerData.trackExitForce.y, direction, player.FacingDirection);

        LeanTween.delayedCall(playerData.trackEffectsDisableDelay, () => {
            player.TrackEffects.Disable();
        });

        LeanTween.scale(player.gameObject, Vector3.one, playerData.trackPlayerExitScaleTime)
            .setEase(LeanTweenType.easeSpring)
            .setDelay(playerData.trackPlayerExitScaleDelay);

        if (!sfxFadeStarted)
        {
            MasterAudio.FadeOutAllOfSound("sfx_electric_current", playerData.trackSfxFadeTime);
            MasterAudio.PlaySoundAndForget("sfx_electric_current_end", playerData.trackSfxEndVolume);
        }

        trackExitTime = Time.time;
        timeOnPoint = 0f;
        timeToPoint = 0f;
        sfxFadeStarted = false;
        isMovingToEnterPoint = true;
        isAbilityDone = true;
        isExitingState = true;
    }

    public bool CanEnterTrack(Collider2D col)
    {
        if (!isAbilityDone) return false;

        if (Time.time - trackExitTime < playerData.trackEnterCooldown) return false;

        Track track = col.GetComponentInParent<Track>();
        TrackEnterPoint enterPoint = track.GetTrackEnterPointForTransform(col.name);

        lastTrackHashCode = track.GetHashCode();

        if (enterPoint == null) return false;

        currentTrack = enterPoint.track;
        trackPoint = enterPoint;
        targetTrackPoint = enterPoint.GetTarget(currentTrack);

        return true;

    }

}
