using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        GameObject.Instantiate(playerData.deathParticlesPrefab, player.transform.position, Quaternion.identity);
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
        }
    }

    public void ResetLevel()
    {

    }



}
