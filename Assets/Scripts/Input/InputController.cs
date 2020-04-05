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

    protected float centerScreenX = 0;

    // Update is called once per frame
    void Update()
    {
        centerScreenX = (Screen.width/2) / Screen.dpi;
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>() / Screen.dpi;
        Vector2 delta = new Vector2(mousePosition.x - centerScreenX, mousePosition.y);
        player.mousePos = delta; 
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (context.started)
            player.Jump();
    }

    public void OnW(InputAction.CallbackContext context)
    {
        player.Boost();
    }
}
