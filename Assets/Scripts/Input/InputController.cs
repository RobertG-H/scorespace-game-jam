using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;  
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    [SerializeField]
    PlayerController player;

    public Text debugText;

    Vector2 mousePosition; 

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = string.Format("Mouse delta: {0}\n Roll Delta: {1}\nVel: {2}", player.MouseDelta, player.rollAngleDelta, player.velocity );
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        Vector2 delta = new Vector2(mousePosition.x - player.CenterScreenX, mousePosition.y);
        player.MouseDelta = delta; 
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
    }
}
