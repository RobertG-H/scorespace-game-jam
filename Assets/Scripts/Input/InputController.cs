using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;  


public class InputController : MonoBehaviour
{
    [SerializeField]
    PlayerController player =null;
    Vector2 mousePosition; 

    // Update is called once per frame
    void Update()
    {
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        Vector2 delta = new Vector2(mousePosition.x - player.CenterScreenX, mousePosition.y);
        player.MouseDelta = delta; 
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        player.Boost();
    }
}
