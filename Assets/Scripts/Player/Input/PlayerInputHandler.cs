using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }
    public Vector2 SnappedMovementInput { get; private set; }

    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    public bool GrabInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool DashInputStop { get; private set; }

    public PlayerColorManager ColorManager;

    [SerializeField]
    private float inputHoldTime = 0.2f;
    private float jumpInputStartTime;
    private float dashInputStartTime;

    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();
        SnappedMovementInput = SnapVector2Dir(RawMovementInput);
        print($"{RawMovementInput}, {SnapVector2Dir(RawMovementInput)}");

        NormInputX = Mathf.RoundToInt(RawMovementInput.x);
        NormInputY = Mathf.RoundToInt(RawMovementInput.y);
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpInput = true;
            JumpInputStop = false;
            jumpInputStartTime = Time.time;
        }

        if (context.canceled)
        {
            JumpInputStop = true;
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            DashInput = true;
            DashInputStop = false;
            dashInputStartTime = Time.time;
        }
        else if (context.canceled)
        {
            DashInputStop = true;
        }
    }

    public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GrabInput = true;
        }

        if (context.canceled)
        {
            GrabInput = false;
        }
    }

    public void OnPaletteRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ColorManager.UpdateForward();
        }
    }

    public void OnPaletteLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ColorManager.UpdateBackward();
        }
    }

    public void UseJumpInput() => JumpInput = false;

    public void UseDashInput() => DashInput = false;

    private void CheckJumpInputHoldTime()
    {
        if (Time.time - jumpInputStartTime >= inputHoldTime)
        {
            JumpInput = false;
        }
    }

    private void CheckDashInputHoldTime()
    {
        if (Time.time - dashInputStartTime >= inputHoldTime)
        {
            DashInput = false;
        }
    }

    private float axisDead = 0.4F;    //radius of axis dead, from 0 to zero
    private float smoothConst = -7.5F;    //some factor that controls smoothing, everything less than -7.5 makes values overshoot
    private bool axisScaleFromZero = true;

    private Vector2 SnapVector2Dir(Vector2 raw)
    {

        Vector2 vec = raw;

        float sign = Mathf.Sign(raw.x * Vector2.up.y - raw.y * Vector2.up.x);
        float angle = Vector2.Angle(Vector2.up, raw) * sign;

        float clampedAngle = Mathf.Round(angle / 45F) * 45F;

        vec = new Vector2(Mathf.Sin(clampedAngle * Mathf.Deg2Rad), Mathf.Cos(clampedAngle * Mathf.Deg2Rad)).normalized * raw.magnitude;

        //Magnitude Clamping and Dead
        float mag = vec.magnitude;
        if (mag < axisDead)
        {
            vec = Vector3.zero;
        }
        else
        {
            if (axisScaleFromZero) { vec = vec.normalized * Mathf.InverseLerp(axisDead, 1F, mag); }    //start from zero after exceeding axis dead
            else { vec = vec.normalized * Mathf.Clamp(mag, axisDead, 1F); }    //start from axis dead
        }

        return vec;

    }
}
