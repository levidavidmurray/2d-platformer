using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;
using DarkTonic.MasterAudio;

public class PlayerDeathState : PlayerState
{

    private float deathStartedAt;
    private bool hasPermissionToDie = false;

    public PlayerDeathState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        LeanTween.delayedCall(playerData.deathColliderKillDelay, () => {
            deathStartedAt = Time.time;

            GameObject.Instantiate(playerData.deathParticlesPrefab, player.transform.position, Quaternion.identity);

            player.Sprite.forceRenderingOff = true;
            player.RB.simulated = false;
            
            LeanTween.value(playerData.playerLightIntensity, 0f, playerData.playerLightFadeTime)
                .setOnUpdate((float value) => {
                    player.Light.intensity = value;
                });

            ProCamera2DShake.Instance.Shake("PlayerHit");
            MasterAudio.PlaySoundAndForget("sfx_death", playerData.deathSfxVolume);

            UIManager.TransitionUI.swipeInOut(
                playerData.deathResetTransitionDuration, 
                playerData.deathResetTransitionDelay,
                playerData.deathResetTransitionHoldTime
            );
            hasPermissionToDie = true;
        });

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.FreezeVelocity();

        if (!hasPermissionToDie || isExitingState) return;

        if (Time.time - deathStartedAt >= playerData.deathResetDelay)
        {
            player.transform.position = player.spawnPoint.position;
            ProCamera2D.Instance.CenterOnTargets();

            if (Time.time - deathStartedAt >= playerData.respawnDelay)
            {
                // Spawn player
                hasPermissionToDie = false;
                SpawnPlayer();
            }
        }
    }

    public void SpawnPlayer()
    {
        player.transform.position = player.spawnPoint.position;
        player.transform.localScale = Vector3.zero;
        player.RB.simulated = false;
        player.Sprite.forceRenderingOff = false;

        LeanTween.value(0f, playerData.playerLightIntensity, playerData.playerLightFadeTime)
            .setOnUpdate((float value) => {
                player.Light.intensity = value;
            });

        player.transform.LeanScale(Vector3.one, playerData.spawnScaleDuration)
            .setEase(LeanTweenType.easeSpring)
            .setOnComplete(() => {
                LeanTween.delayedCall(playerData.spawnDuration, () => {
                    player.RB.simulated = true;
                    stateMachine.ChangeState(player.IdleState);
                });
            });
    }

    public override void Die()
    {
        // Already Dead
    }

}
