using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Debug Config")]
    public bool logStateTransition = false;
    public bool useDashPitch = false;

    [Header("Move State")]
    public float movementVelocity = 10f;

    [Header("Jump State")]
    public float jumpVelocity = 15f;
    public int amountOfJumps = 2;

    [Header("Dash State")]
    public float dashVelocity = 20f;
    public float dashTime = 0.5f;
    public float dashCooldown = 0.5f;
    public bool hasAfterImage = true;
    public bool hasTrail = true;
    public float distBetweenAfterImages = 0.5f;
    public float afterImageAlphaSet = 0.8f;
    public float afterImageAlphaDecay = 0.85f;
    public float dashScale = 0.1f;
    public float dashScaleTime = 0.2f;
    public float dashExitScaleDelay = 0.2f;
    public float dashPitch = 1f;
    public float[] dashPitchVariants = new[] {1.1f, 1.2f, 1.3f, 1.4f};
    [Header("Dash SFX")]
    public float dashDelayForSfx = 0.2f;
    public float dashSfxVolume = 0.65f;

    [Header("Wall Jump State")]
    public float wallJumpVelocity = 20f;
    public float wallJumpTime = 0.4f; // Time to stay in wallJump state to prevent moving back to wall
    public Vector2 wallJumpAngle = new Vector2(1, 2);

    [Header("In Air State")]
    public float coyoteTime = 0.2f;
    public float variableJumpHeightMultiplier = 0.5f;

    [Header("Wall Slide State")]
    public float wallSlideVelocity = 3f;

    [Header("Wall Climb State")]
    public float wallClimbVelocity = 3f;

    [Header("Ledge Climb State")]
    public Vector2 startOffset;
    public Vector2 stopOffset;

    [Header("Track State")]
    public float trackVelocity = 16f;
    public Vector2 trackExitForce = new Vector2(0f, 16f);
    public float trackEffectsDisableDelay = 0.1f;
    public Vector3 trackPlayerScale = new Vector3(0.1f, 0.1f, 0.1f);
    public float trackPlayerExitScaleTime = 0.2f;
    public float trackPlayerExitScaleDelay = 0.2f;
    public float trackPlayerEnterScaleTime = 0.2f;
    public float trackSfxFadeExitTime = 0.5f; // Ms before exiting track to fade sfx
    public float trackSfxFadeTime = 0.4f;
    public float trackSfxVolume = 0.5f;
    public float trackSfxPitch = 1f;
    public float trackEnterCooldown = 0.1f;
    public float trackHorizontalExitGravityDelay = 0.15f;

    [Header("Check Variables")]
    public float groundCheckRadius = 0.3f;
    public float wallCheckDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("Death State")]
    public float deathColliderKillDelay = 0.05f;
    public float deathResetDelay = 0.5f; // How long to wait before resetting player
    public float deathResetTransitionDuration = 1f; // Duration of transition tween
    public float deathResetTransitionHoldTime = 0.75f; // How long to hold black screen
    public float deathResetTransitionDelay = 0.25f; // How long to wait before transitioning
    public float respawnDelay = 3f; // How long to wait to respawn after transitioning to spawn point
    public float spawnScaleDuration = 0.75f; // How long scale tween takes to finish upon respawning
    public float spawnDuration = 1.25f; // How long to wait after respawn starts until player receives control
    public GameObject deathParticlesPrefab;
    public Vector2 deathParticlesOffset;
    public float deathSfxVolume = 0.65f;
    public float playerLightFadeTime = 0.3f;
    public float playerLightIntensity = 0.45f;
}
