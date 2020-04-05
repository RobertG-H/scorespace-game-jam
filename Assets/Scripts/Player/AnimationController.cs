using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    public Transform board;
    public Transform hips;
    public Transform head;
    public Transform LHandIK;
    public Transform pizzaBoxBase;
    public Transform LHandTargetForward;
    public Transform LHandTargetBackward;
    public float headRotationOffset;
    public float headTracking; 
    private Vector3 initHipsPos;
    private Vector3 initLHandIKPos;

    private Vector3 initpizzaBoxBasePos;

    // Start is called before the first frame update
    void Start()
    {
        initHipsPos = hips.localPosition;
        initLHandIKPos = LHandIK.localPosition;
        initpizzaBoxBasePos = pizzaBoxBase.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
        float boardAngleX = board.eulerAngles.x;
        float boardAngleZ = board.eulerAngles.z;

        if(boardAngleX > 180)
        {
            boardAngleX -= 360;
        }
        if(boardAngleZ > 180)
        {
            boardAngleZ -= 360;
        }
        

        //Hip positioning
        hips.rotation = board.rotation;
        Vector3 newHipsPos = hips.localPosition;
        newHipsPos.y = initHipsPos.y * Mathf.Cos(boardAngleX*Mathf.Deg2Rad);
        hips.localPosition = newHipsPos;

        //Head positioning  
        Quaternion newHeadRotation = Quaternion.identity;
        newHeadRotation *= Quaternion.Euler(0, (board.eulerAngles.x+headRotationOffset), 0);
        head.localRotation = Quaternion.Lerp(head.localRotation, newHeadRotation, headTracking);

        //Right hand positioning
        Vector3 newPizzaBoxBasePos = pizzaBoxBase.localPosition;
        newPizzaBoxBasePos.y = initpizzaBoxBasePos.y *(-0.2f/30f*Mathf.Abs(boardAngleX) + 1);

        if(boardAngleX > 0)
        {
            newPizzaBoxBasePos.z = initpizzaBoxBasePos.y - newPizzaBoxBasePos.y;
        }
        else
        {
            newPizzaBoxBasePos.z = -(initpizzaBoxBasePos.y - newPizzaBoxBasePos.y);
        }

        if(boardAngleZ > 0)
        {
            newPizzaBoxBasePos.x = initpizzaBoxBasePos.x - initpizzaBoxBasePos.x * Mathf.Abs(Mathf.Sin(boardAngleZ*Mathf.Deg2Rad));
            newPizzaBoxBasePos.y = newPizzaBoxBasePos.y + initpizzaBoxBasePos.y * Mathf.Abs(Mathf.Sin(boardAngleZ*Mathf.Deg2Rad))*0.5f;

        }
        else
        {
            newPizzaBoxBasePos.x = initpizzaBoxBasePos.x + initpizzaBoxBasePos.x * Mathf.Abs(Mathf.Sin(boardAngleZ*Mathf.Deg2Rad))*0.5f;
            newPizzaBoxBasePos.y = newPizzaBoxBasePos.y - initpizzaBoxBasePos.y * Mathf.Abs(Mathf.Sin(boardAngleZ*Mathf.Deg2Rad))*0.5f;
        }        
        pizzaBoxBase.localPosition = newPizzaBoxBasePos;
        
        //Left hand positioning
        if(boardAngleX > 0)
        {
            LHandIK.localPosition = Vector3.Lerp(initLHandIKPos, LHandTargetForward.localPosition, Mathf.Abs(boardAngleX/30f));
        }
        else
        {
            LHandIK.localPosition = Vector3.Lerp(initLHandIKPos, LHandTargetBackward.localPosition, Mathf.Abs(boardAngleX/30f));
        }
    }
}
