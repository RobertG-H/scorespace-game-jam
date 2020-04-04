﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    enum RollDirection {None, Left, Right};

    CharacterController characterController;
    public Transform board;
    public PizzaBoxManager pizzaBoxManager;
    public float acceleration;
    public float gravity;
    public float speed = 0f;
    private Vector3 velocity;
    private float prevRollAngleRad = 0f;
    public float maxRollAngleDeg;
    public float maxTurnAngleDeg;
    public float rollDeltaSlowThreshold;
    public bool isSkidding = false;
    private bool isGrounded = false;
    public float RAYCASTDOWNDIST;
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

    RollDirection rollSide
    {
        get
        {
            if (rollAngleRad == 0)
                return RollDirection.None;
            else if (rollAngleRad > 0)
                return RollDirection.Right;
            return RollDirection.Left;
        }
    }


    // Debuging tools
    public bool isDebug;
    public Text debugText;
    public Image debugImage;

    public float rollAngleDeltaDeadzone;
    public float rollAngleDelta;
    // In degrees
    // 0 Angle is in direction of z-axis (straight up)
    protected float rollAngleRad
    {
        get
        {
            float currentAngle = Mathf.Atan2(mouseDelta.x, Screen.height);
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
    protected float centerScreenX = 0;
    public float CenterScreenX
    {
        get
        {
            return centerScreenX;
        }
    }

    protected Vector2 mouseDelta;
    public Vector2 MouseDelta
    {
        get
        {
            return mouseDelta;
        }
        set
        {
            mouseDelta = value;
        }
    }

    void Awake()
    {
        DebugGUI.SetGraphProperties("rollDeltaGraph", "RollDelta", -2f, 2f, 3, new Color(1, 1, 0), false);
        DebugGUI.SetGraphProperties("velGraph", "Vel", 0, 120, 2, new Color(1, 0.5f, 1), false);

    }

    void Start()
    {
        centerScreenX = Screen.width/2;
        characterController = GetComponent<CharacterController>();
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
        if(Mathf.Abs(d) < rollAngleDeltaDeadzone )
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
    }


    void UpdateVelocity()
    {
        Vector3 moveDirection = transform.forward;
        float dampVel = 0f;

        // Increase acceleration if rolling in direction of turn
        if (rollingDirection != RollDirection.None && rollSide == rollingDirection)
        {
            speed += acceleration * Mathf.Abs(rollAngleRad);
        }
        // Slowdown stuff
        if (!isSkidding && rollAngleDelta > rollDeltaSlowThreshold)
        {
            isSkidding = true;
            Skid();
        }
        if ( rollAngleDelta < rollDeltaSlowThreshold)
        {
            isSkidding = false;
        }
        // // No delta on the roll then smoothdamp to stop
        // if (rollAngleDeg == 0)
        // {
        //     velocity = Mathf.SmoothDamp(velocity, 0f, ref dampVel, Mathf.Abs(rollAngleRad+1f)*2f);
        // }
        // else // Constantly slowing down based on roll angle
        // {
        //     velocity -= Mathf.Lerp(velocity*0.01f,0, Mathf.Abs(rollAngleDeg)/maxRollAngleDeg);
        // }

        velocity = moveDirection * speed;
        if (!RayCastGround()) 
        {
            velocity.y += gravity; 
        }
        else
        {
            velocity.y = 0f;
        }

        // isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        // if (isGrounded && _velocity.y < 0)
        //     _velocity.y = 0f;

        characterController.Move(velocity*Time.deltaTime);
    }

    void Skid()
    {
        float overshoot = (rollAngleDelta - rollDeltaSlowThreshold) / rollDeltaSlowThreshold;
        speed -= Mathf.Lerp(0,speed/1f, overshoot); 
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
        if (speed < 15)
        {
            speed = 15;
        }
    }

    private bool RayCastGround()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        layerMask += 1 << LayerMask.NameToLayer("Player");
        layerMask = ~layerMask;
        Vector3 currentPos = transform.position;

        if (Physics.Raycast(currentPos, transform.TransformDirection(Vector3.down), RAYCASTDOWNDIST, layerMask))
        {
            Debug.DrawRay(currentPos, transform.TransformDirection(Vector3.down) * RAYCASTDOWNDIST, Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(currentPos, transform.TransformDirection(Vector3.down) * RAYCASTDOWNDIST, Color.yellow);
            return false;
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

        Debug.Log("HIT");
        speed = 0f;


        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        // Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pizza");
        if(other.gameObject.tag == "Pizza Pickup")
        {
            int numPizzas = other.gameObject.GetComponent<PizzaPickupController>().numberOfPizzas;
            pizzaBoxManager.AddPizzaBox(numPizzas);

            Destroy(other.gameObject);
        }

    }

    void UpdateLogger()
    {
        DebugGUI.Graph("rollDeltaGraph",  rollAngleDelta);
        DebugGUI.Graph("velGraph", speed);
        debugText.text = string.Format("Grounded: {0}\n Vel: {1}\nCollide: {2}", RayCastGround(), velocity, characterController.collisionFlags);
    }
}
