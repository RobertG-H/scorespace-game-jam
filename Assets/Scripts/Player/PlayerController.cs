﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    enum RollDirection {None, Left, Right};
    CharacterController characterController;
    public Transform board;
    public Vector2 mousePos { get; set; }

	[SerializeField]
	private bool isGrounded = false;

    // VELOCITY
    public float MOVEMENTACCEL;
    public float GRAVITY;
    [HideInInspector]
    public float speed = 0f;
    private Vector3 velocity;
    private Vector3 lastGroundVel;

    // SKIDDING
    public float SKIDTHRESHOLD;
    private bool isSkidding = false;
    public float SKIDSPEEDLOSS;
    
    // JUMPING
    private bool isJumping = false;
    private float currentJumpTime;
    public float JUMPTIME;
    public float JUMPFORCE;


    // ANGLES
    private float prevRollAngleRad = 0f;
    public float maxRollAngleDeg;
    public float maxTurnAngleDeg;
    public float baseRaycastDistHeight;
    public float ROLLANGLEDELTADEADZONE;
    public float rollAngleDelta {get; private set;}



    RollDirection rollingDirection
    {
        get
        {
            if (rollAngleDelta == 0)
                return RollDirection.None;
            else if (rollAngleDelta > 0)
                return RollDirection.Right;
            return RollDirection.Left;
        }
    }

    // In degrees
    // 0 Angle is in direction of z-axis (straight up)
    protected float rollAngleRad
    {
        get
        {
            float currentAngle = Mathf.Atan2(mousePos.x, Screen.height/Screen.dpi);
            if (Mathf.Abs(currentAngle) > maxRollAngleDeg * Mathf.Deg2Rad) currentAngle = maxRollAngleDeg * Mathf.Deg2Rad * Mathf.Sign(currentAngle);
            return currentAngle;
        }

    }

    protected float rollAngleDeg
    {
        get
        {
            return Mathf.Rad2Deg * rollAngleRad;
        }
    }

    // Debuging tools
    public bool isDebug;
    public Text debugText;
    public Image debugImage;

    void Awake()
    {
        if (isDebug)
        {
            DebugGUI.SetGraphProperties("rollDeltaGraph", "RollDelta", 0f, SKIDTHRESHOLD, 3, new Color(1, 1, 0), false);
            DebugGUI.SetGraphProperties("velGraph", "Vel", 0, 120, 2, new Color(1, 0.5f, 1), false);
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        lastGroundVel = transform.forward;
        currentJumpTime = 0;
    }


    void Update()
    {
        UpdateRollAngleDelta();
        UpdateRotation();
        UpdateVelocity();
        if (isDebug) UpdateLogger();
    }

    void UpdateRollAngleDelta()
    {
        float d = (rollAngleRad - prevRollAngleRad) / Time.deltaTime;
        if(Mathf.Abs(d) < ROLLANGLEDELTADEADZONE )
            rollAngleDelta = 0f;
        else
            rollAngleDelta = d;
        prevRollAngleRad = rollAngleRad;
    }

    void UpdateRotation()
    {
        // Update player yaw based on rollangle
        float yawUpdate = Mathf.Lerp(0,maxTurnAngleDeg,Mathf.Abs(rollAngleDeg)/maxRollAngleDeg) * Mathf.Sign(rollAngleDeg); 
        transform.Rotate(0,yawUpdate,0,Space.World);

        // Update board roll
        board.rotation = Quaternion.Euler(rollAngleDeg, board.rotation.eulerAngles.y, board.rotation.eulerAngles.z);

        // Update board pitch
        RaycastHit hit = RayCastGround();
        // Return if in midair
        if (hit.normal == new Vector3(0,0,0)) return;
        // Debug.DrawRay(transform.position, hit.normal * 10.0f, Color.yellow,100f);
        // Debug.DrawRay(transform.position, transform.forward * 10.0f, Color.red,100f);
        Vector3 normalDir = Vector3.Normalize(hit.normal);
        Vector3 forwardDir = Vector3.Normalize(transform.forward);
        float targetAngle = (Mathf.Acos(Vector3.Dot(normalDir, forwardDir)) * Mathf.Rad2Deg -90) *-1;
        
        float sourceAngle = board.rotation.eulerAngles.z;
        float singleStep = 10f * Time.deltaTime;

        float moveAngle = Mathf.LerpAngle(sourceAngle,targetAngle, singleStep);
        board.rotation = Quaternion.Euler(board.rotation.eulerAngles.x, board.rotation.eulerAngles.y, moveAngle);
    }


	void UpdateVelocity()
	{

		// Increase acceleration if rolling in any direction
		if (rollingDirection != RollDirection.None)
		{
			speed += MOVEMENTACCEL * Mathf.Abs(rollAngleRad);
		}



		Vector3 lastDirection = characterController.velocity.normalized;
		Vector3 lastVelocity = characterController.velocity;
		if (characterController.velocity.sqrMagnitude <= 0f)
			lastDirection = this.transform.forward;



		RaycastHit hit = RayCastGround();
		Vector3 normal = hit.normal;
		bool rayIsGrounded = normal != Vector3.zero;

		float groundDotHit = Vector3.Dot(normal, lastDirection);

		this.isGrounded = rayIsGrounded && groundDotHit <= 0.01f;
		if (!this.isGrounded)
			Debug.Log(rayIsGrounded + " << rayhit      groundDotHit > " + (groundDotHit <= 0) + "\n " + groundDotHit);

		// Calculate ground or air movement
		if (this.isGrounded)
        {
			Vector3 groundMovementDirection = Vector3.ProjectOnPlane(transform.forward, normal).normalized;
			//Debug.Log("grndmvmntDir " + groundMovementDirection);

			// Slowdown stuff
			if (!isSkidding && Mathf.Abs(rollAngleDelta) > SKIDTHRESHOLD)
            {
                isSkidding = true;
                Skid();
            }
            if ( Mathf.Abs(rollAngleDelta) < SKIDTHRESHOLD)
            {
                isSkidding = false;
            }

            velocity = groundMovementDirection* speed;
        }
        else
        {
            velocity = lastGroundVel;
            //lastGroundVel.y += GRAVITY;
        }

        // Always add gravity
        velocity.y += GRAVITY;

		if (isJumping)
		{
			isJumping = false;
			velocity.y = 50f;
		}


		Vector3 frameWiseVelocity = velocity*Time.deltaTime;

        
        characterController.Move(frameWiseVelocity);

		// Constantly record your ground speed and direction incase you go off an edge.
		//if(this.isGrounded)
		//{
		//    lastGroundVel = characterController.velocity;
		//    isJumping = false;
		//}

		lastGroundVel = velocity;

	}

    void Skid()
    {
        speed -= speed * SKIDSPEEDLOSS;
        if (isDebug)
        {
            if (debugImage.color == Color.white)
                debugImage.color = Color.cyan;
            else
                debugImage.color = Color.white;
        }
    }

    public void Boost()
    {
        if (speed < 100)
        {
            speed = 100;
        }
    }

    public void Jump()
    {
        if (isJumping) return;
        if (!this.isGrounded) return;
        isJumping = true;
    }

    private RaycastHit RayCastGround()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        layerMask += 1 << LayerMask.NameToLayer("Player");
        layerMask = ~layerMask;
        Vector3 currentPos = transform.position;

		RaycastHit hit;

		float raycastDistBasedOnAngle = Mathf.Abs(1f/-this.transform.up.y);
		raycastDistBasedOnAngle = baseRaycastDistHeight * raycastDistBasedOnAngle;


        if (Physics.Raycast(currentPos, -transform.up, out hit, raycastDistBasedOnAngle, layerMask))
        {
            Debug.DrawRay(currentPos, -transform.up* hit.distance, Color.red);
            return hit;
        }
        else
        {
            Debug.DrawRay(currentPos, -transform.up* 5.0f, Color.yellow);
            return hit;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {

        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }
        // Return if not touching sides
        if (characterController.collisionFlags != CollisionFlags.Sides) return;
        // speed = 0f;
    }

    void UpdateLogger()
    {
        DebugGUI.Graph("rollDeltaGraph",  Mathf.Abs(rollAngleDelta));
        DebugGUI.Graph("velGraph", speed);
        debugText.text = string.Format("Grounded: {0}\n rollAngleDelta: {1}\n Overshoot: {2}", characterController.isGrounded, rollAngleDelta,  (rollAngleDelta - SKIDTHRESHOLD) / SKIDTHRESHOLD);
    }
}
