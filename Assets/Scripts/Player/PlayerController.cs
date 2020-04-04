using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    public Transform board;
    private float acceleration = 0.1f;
    public float velocity = 0f;
    private float prevRollAngleRad;
    public float maxRollAngleDeg;
    public float maxTurnAngleDeg;


    // Debuging tools
    public bool isDebug;
    public Text debugText;

    public float rollAngleDelta
    {
        get
        {
            return rollAngleRad - prevRollAngleRad;
        }
    }
    // In degrees
    // 0 Angle is in direction of z-axis (straight up)
    protected float rollAngleRad
    {
        get
        {
            return Mathf.Atan2(mouseDelta.x, mouseDelta.y) * -1;
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


    void Start()
    {
        centerScreenX = Screen.width/2;
        characterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        UpdateRotation();
        UpdateVelocity();
        if (isDebug) UpdateLogger();
        prevRollAngleRad = rollAngleRad;
    }

    void UpdateRotation()
    {
        // Update player yaw based on rollangle
        float yawUpdate = Mathf.Lerp(0,maxTurnAngleDeg,Mathf.Abs(rollAngleDeg)/maxRollAngleDeg) * Mathf.Sign(rollAngleDeg) * -1; 
        transform.Rotate(0,yawUpdate,0,Space.World);

        // Update board roll
        float rollUpdate = rollAngleDeg * -1;
        if (Mathf.Abs(rollUpdate) > maxRollAngleDeg) rollUpdate = maxRollAngleDeg * Mathf.Sign(rollUpdate);
        board.rotation = Quaternion.Euler(rollUpdate, board.rotation.eulerAngles.y, board.rotation.eulerAngles.z);
    }


    void UpdateVelocity()
    {
        Vector3 moveDirection = transform.forward;
        float dampVel = 0f;
        if (rollAngleDelta != 0)
            velocity += acceleration * Mathf.Abs(rollAngleRad);
        else
        {
            velocity = Mathf.SmoothDamp(velocity, 0f, ref dampVel, Mathf.Abs(rollAngleRad+1f)*2f);
        }
        characterController.Move(moveDirection * velocity * Time.deltaTime);
    }


    void UpdateLogger()
    {
        debugText.text = string.Format("Mouse delta: {0}\n Roll Delta: {1}\nVel: {2}", MouseDelta, rollAngleDelta, velocity);
    }
}
