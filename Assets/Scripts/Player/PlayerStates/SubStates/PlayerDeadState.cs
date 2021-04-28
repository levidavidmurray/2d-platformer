using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class PlayerDeadState : PlayerState
{

    private float deathStartedAt;

    public PlayerDeadState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        deathStartedAt = Time.time;
        player.Sprite.forceRenderingOff = true;

        ProCamera2DShake.Instance.Shake("PlayerHit");

        GameObject.Instantiate(playerData.deathParticlesPrefab, player.transform.position, Quaternion.identity);
        UIManager.TransitionUI.swipeInOut(
            playerData.deathResetTransitionDuration, 
            playerData.deathResetTransitionDelay,
            playerData.deathResetTransitionHoldTime
        );
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.FreezeVelocity();

        if (Time.time - deathStartedAt >= playerData.deathResetDelay)
        {
            player.transform.position = player.spawnPoint.position;

            if (Time.time - deathStartedAt >= playerData.respawnDelay)
            {
                this.SpawnPlayer();
            }
        }
    }

    public void SpawnPlayer()
    {
        player.Sprite.forceRenderingOff = false;
        stateMachine.ChangeState(player.IdleState);
    }

    public override void Die()
    {
        // Already Dead
    }

}
