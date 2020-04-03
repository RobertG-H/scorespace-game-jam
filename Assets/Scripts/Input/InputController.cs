using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
  
    CharacterController characterController;

    float moveSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the controller
        Vector3 moveDirection = Vector3.forward * moveSpeed;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        Debug.Log("Mouse");
        Debug.Log(context.ReadValue<Vector2>());
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        Debug.Log("Space Button");

        Debug.Log(context.ReadValue<float>());
        moveSpeed = context.ReadValue<float>();
    }
}
