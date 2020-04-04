using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    CharacterController characterController;

    private float acceleration = 0.1f;

    public float velocity = 0f;


    private float prevRollAngleRad;

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


    // Start is called before the first frame update
    void Start()
    {
        centerScreenX = Screen.width/2;
        characterController = GetComponent<CharacterController>();
    }

    

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rollAngleDeg);
        transform.Rotate(0,rollAngleRad*-1,0,Space.World);
        Vector3 moveDirection = transform.forward;
        Debug.DrawRay(transform.position, moveDirection, Color.green);

        float dampVel = 0f;
        if (rollAngleDelta != 0)
            velocity += acceleration * Mathf.Abs(rollAngleRad);
        else
        {
            // velocity -= Mathf.SmoothDamp(velocity, 0f, ref dampVel, Mathf.Abs(1/rollAngleRad)*100f);
        }
        // velocity -= Mathf.SmoothDamp(velocity, 0f, ref dampVel, Mathf.Abs(1/rollAngleRad)*100f);
        characterController.Move(moveDirection * velocity * Time.deltaTime);

        prevRollAngleRad = rollAngleRad;
    }
}
