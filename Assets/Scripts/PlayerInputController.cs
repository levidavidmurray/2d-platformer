using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerInputController : MonoBehaviour
{
	// movement config
	public float defaultGravity = -25f;
	public float wallSlideGravity = -10f;
	public float wallHitVerticalVelocity = -2f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;
	public float airJumpThresholdSeconds = 0.1f;
	public float wallJumpThresholdSeconds = 0.1f;

	[Header("Prefabs")]
	public GameObject dustParticles_prefab;
	public GameObject wallSlideDust;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;
	private float _gravity = -25f;
	private bool _isWallSliding = false;
	private float _airJumpAttemptTime = 0f;
	private float _wallLeaveTime = 0f;

	void Awake()
	{
		//_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;

		_gravity = defaultGravity;

		wallSlideDust.SetActive(false);
	}


	#region Event Listeners

	void onControllerCollider(RaycastHit2D hit)
	{
		// bail out on plain old ground hits cause they arent very interesting
		if (hit.normal.y == 1f)
        {
			return;
        }

		// reduce vertical velocity on wall hit
		if (Mathf.Abs(hit.normal.x) == 1f && !_isWallSliding)
        {
			_isWallSliding = true;
        }


		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent(Collider2D col)
	{
		Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
	}


	void onTriggerExitEvent(Collider2D col)
	{
		Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
        if (_controller.collisionState.becameGroundedThisFrame)
        {
            print($"velocity.y 1: {_controller.velocity.y}");
        }

		if (_controller.isGrounded)
        {
            if (_controller.collisionState.becameGroundedThisFrame)
            {
                print($"velocity.y 2: {_controller.velocity.y}");
            }

			_velocity.y = 0;
        }

		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");

		if (x > 0)
		{
			normalizedHorizontalSpeed = 1;
			if (transform.localScale.x < 0f)
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

			if (_controller.isGrounded)
            {
				//_animator.Play(Animator.StringToHash("Run"));
            }
		}
		else if (x < 0)
		{
			normalizedHorizontalSpeed = -1;
			if (transform.localScale.x > 0f)
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

			if (_controller.isGrounded)
            {
				//_animator.Play(Animator.StringToHash("Run"));
            }
		}
		else
		{
			normalizedHorizontalSpeed = 0;
		}

		bool isWallSlidingThisFrame = IsWallSlide(x);

		if (_isWallSliding && !isWallSlidingThisFrame)
        {
			_wallLeaveTime = Time.time;
        }

		if (Input.GetButtonDown("Jump"))
        {
			if (_controller.isGrounded)
            {
				Jump();
            }
			else
            {
				_airJumpAttemptTime = Time.time;

				if (Time.time - _wallLeaveTime <= wallJumpThresholdSeconds)
                {
					Jump();
                }
            }
        }

		if (_controller.collisionState.becameGroundedThisFrame)
        {
			if (Time.time - _airJumpAttemptTime <= airJumpThresholdSeconds)
            {
				Jump();
            }
        }


		_isWallSliding = isWallSlidingThisFrame;
		float gravityThisFrame = _gravity;

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

		if (_velocity.y > 0 && !Input.GetKey(KeyCode.UpArrow))
        {
			// rising up and let go of jump key
			gravityThisFrame *= (lowJumpMultiplier - 1);
        }
		else
        {
            gravityThisFrame *= (fallMultiplier - 1);
        }

		if (_isWallSliding)
        {
			if (_velocity.y < 0)
            {
				gravityThisFrame = wallSlideGravity;
            }

			if (!wallSlideDust.activeSelf)
            {
                wallSlideDust.SetActive(true);
            }
        }
		else
        {
			if (wallSlideDust.activeSelf)
            {
                wallSlideDust.SetActive(false);
            }
        }

		_velocity.y += gravityThisFrame * Time.deltaTime;

		// apply gravity before moving
		//_velocity.y += gravityThisFrame * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
		//if (_controller.isGrounded && Input.GetKey(KeyCode.DownArrow))
		//{
		//	_velocity.y *= 3f;
		//	_controller.ignoreOneWayPlatformsThisFrame = true;
		//}

		_controller.move(_velocity * Time.deltaTime);

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

	}

	private void Jump()
    {
        _velocity.y = Mathf.Sqrt(2f * jumpHeight * -_gravity);

		if (_controller.isGrounded)
        {
            Instantiate(dustParticles_prefab, transform.position, Quaternion.identity);
        }
    }

	/**
	 * @param x - player's horizontal input
	 **/
	private bool IsWallSlide(float x)
    {
		return ((_controller.collisionState.left && x < 0) || 
			(_controller.collisionState.right && x > 0)) && !_controller.isGrounded;
    }

	private bool IsOnWall()
    {
		return _controller.collisionState.left || _controller.collisionState.right;
    }

}
