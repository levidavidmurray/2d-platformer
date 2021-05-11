using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public static class GameTrigger
{
    public const string DEATH_COLLIDER = "DeathCollider";
    public const string SPAWN_POINT = "SpawnPoint";
    public const string TRACK_ENTER = "TrackEnter";
}

public class Player : MonoBehaviour
{
    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallGrabState WallGrabState { get; private set; }
    public PlayerWallClimbState WallClimbState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerDeathState DeathState { get; private set; }
    public PlayerTrackState TrackState { get; private set; }

    [SerializeField]
    private PlayerData playerData;
    #endregion

    #region Components
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public SpriteRenderer Sprite { get; private set; }
    public TrailRenderer Trail { get; private set; }
    public Light2D Light { get; private set; }
    public PlayerTrackEffects TrackEffects { get; private set; }
    #endregion

    #region Other Variables
    public Vector2 CurrentVelocity { get; private set; }
    public int FacingDirection { get; private set; }

    private Vector2 workspace;
    public Transform spawnPoint;
    public int playerLightPingPongTweenId;
    public int playerLightFadeTweenId;

    private float lightPingPongIntensityMax;
    private float lightPingPongIntensityMin;
    private float lightPingPongTime;
    #endregion

    #region Check Transforms

    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;

    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        StateMachine = new PlayerStateMachine(playerData.logStateTransition);
        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        DashState = new PlayerDashState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, playerData, "wallSlide");
        WallGrabState = new PlayerWallGrabState(this, StateMachine, playerData, "wallGrab");
        WallClimbState = new PlayerWallClimbState(this, StateMachine, playerData, "wallClimb");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, playerData, "inAir");
        DeathState = new PlayerDeathState(this, StateMachine, playerData, "idle");
        TrackState = new PlayerTrackState(this, StateMachine, playerData, "idle");

        // LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, playerData, "ledgeClimbState");
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        Sprite = GetComponent<SpriteRenderer>();
        Trail = GetComponentInChildren<TrailRenderer>();
        Light = GetComponentInChildren<Light2D>();
        TrackEffects = GetComponentInChildren<PlayerTrackEffects>();

        TrackEffects.Disable();

        Trail.emitting = false;

        FacingDirection = 1;
        Sprite.forceRenderingOff = true;
        Light.intensity = 0;

        lightPingPongIntensityMin = playerData.playerLightPingPongIntensityMin;
        lightPingPongIntensityMax = playerData.playerLightPingPongIntensityMax;
        lightPingPongTime = playerData.playerLightPingPongTime;

        LeanTween.delayedCall(playerData.deathResetDelay, () => {
            DeathState.SpawnPlayer();
        });
    }

    private void Update()
    {
        CurrentVelocity = RB.velocity;
        UIManager.DebugUI.OnPlayerVelocityChange(CurrentVelocity);
        StateMachine.CurrentState?.LogicUpdate();

        // TODO: Remove (only for dev debugging)
        if (InputHandler.GrabInput) {
            StateMachine.CurrentState.Exit();
            DeathState.SpawnPlayer();            
        }

        CheckLightPingPongIntensity();

        var pos = transform.position;
        Debug.DrawLine(pos, pos + (Vector3.right * FacingDirection), Color.red);
        Debug.DrawLine(pos, pos + ((Vector3)InputHandler.SnappedMovementInput), Color.green);
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState?.PhysicsUpdate();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        switch(col.tag)
        {
            case GameTrigger.DEATH_COLLIDER:
                StateMachine.CurrentState.Die();
                break;
            case GameTrigger.SPAWN_POINT:
                spawnPoint = col.gameObject.GetComponent<RespawnPointTrigger>().SpawnPoint;
                break;
            case GameTrigger.TRACK_ENTER:
                if (TrackState.CanEnterTrack(col))
                    StateMachine.ChangeState(TrackState);
                break;
        }
    }

    #endregion

    #region Set Methods

    public void FreezeVelocity()
    {
        RB.velocity = Vector2.zero;
        CurrentVelocity = Vector2.zero;
    }

    public void RBResume()
    {
        RB.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
    }

    public void RBFreeze()
    {
        RB.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
    }

    public void RBFreezeX()
    {
        RB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    public void RBFreezeY()
    {
        RB.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }
    #endregion

    #region Check Methods

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.groundLayer);
    }

    public bool CheckIfTouchingWall(Vector2 direction)
    {
        return Physics2D.Raycast(wallCheck.position, direction, playerData.wallCheckDistance, playerData.groundLayer);
    }

    public bool CheckIfTouchingWall()
    {
        return CheckIfTouchingWall(Vector2.right * FacingDirection);
    }

    public bool CheckIfTouchingWallBack()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * -FacingDirection, playerData.wallCheckDistance, playerData.groundLayer);
    }

    public bool CheckIfTouchingLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.groundLayer);
    }

    public void CheckIfShouldFlip(int xInput)
    {
        if (xInput != 0 && xInput != FacingDirection)
        {
            Flip();
        }
    }

    public void CheckLightPingPongIntensity()
    {
        if (
            lightPingPongTime != playerData.playerLightPingPongTime  ||
            lightPingPongIntensityMin != playerData.playerLightPingPongIntensityMin ||
            lightPingPongIntensityMax != playerData.playerLightPingPongIntensityMax
        ) 
        {
            lightPingPongTime = playerData.playerLightPingPongTime;
            lightPingPongIntensityMin = playerData.playerLightPingPongIntensityMin;
            lightPingPongIntensityMax = playerData.playerLightPingPongIntensityMax;

            StartPingPongLight();
        }
    }

    #endregion

    #region Other Methods

    public Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.groundLayer);
        float xDist = xHit.distance;
        workspace.Set(xDist * FacingDirection, 0f);
        RaycastHit2D yHit = Physics2D.Raycast(ledgeCheck.position + (Vector3)(workspace), Vector2.down, ledgeCheck.position.y - wallCheck.position.y, playerData.groundLayer);
        float yDist = yHit.distance;

        workspace.Set(wallCheck.position.x + (xDist * FacingDirection), ledgeCheck.position.y - yDist);

        return workspace;
    }

    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    public void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    public LTDescr StopPingPongLight(float newIntensity, float fadeTime = -1f)
    {
        if (fadeTime < 0)
        {
            fadeTime = playerData.playerLightFadeTime;
        }

        LeanTween.cancel(playerLightPingPongTweenId);
        LeanTween.cancel(playerLightFadeTweenId);
        
        LTDescr tween = LeanTween.value(
            Light.intensity,
            newIntensity,
            fadeTime
        ).setOnUpdate((float intensity) => {
            Light.intensity = intensity;
        });

        playerLightFadeTweenId = tween.id;
        
        return tween;
    }

    public void StartPingPongLight()
    {
        LeanTween.cancel(playerLightPingPongTweenId);
        LeanTween.cancel(playerLightFadeTweenId);

        StopPingPongLight(lightPingPongIntensityMax, playerData.playerLightFadeTime)
            .setOnComplete(() => {
                playerLightPingPongTweenId = LeanTween.value(
                    lightPingPongIntensityMin,
                    lightPingPongIntensityMax,
                    lightPingPongTime
                )
                .setLoopPingPong()
                .setOnUpdate((float intensity) => {
                    Light.intensity = intensity;
                }).id;
            });
    }

    #endregion

}
