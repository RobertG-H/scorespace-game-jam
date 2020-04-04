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

    private Vector3 initHipsPos;
    private Vector3 initLHandIKPos;

    private Vector3 initpizzaBoxBasePos;

    // Start is called before the first frame update
    void Start()
    {
        initHipsPos = hips.localPosition;
        initLHandIKPos = LHandIK.position;
        initpizzaBoxBasePos = pizzaBoxBase.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
        float boardAngle = board.localEulerAngles.x*Mathf.Deg2Rad;

        hips.localRotation = board.localRotation;
        Vector3 newHipsPos = hips.localPosition;
        newHipsPos.y = initHipsPos.y * Mathf.Cos(boardAngle );
        hips.localPosition = newHipsPos;

        float boardAngleDeg = board.localEulerAngles.x;
        if(boardAngleDeg > 180)
        {
            boardAngleDeg -= 360;
        }
        Vector3 newPizzaBoxBasePos = pizzaBoxBase.localPosition;
        newPizzaBoxBasePos.y = initpizzaBoxBasePos.y *(-0.2f/30f*Mathf.Abs(boardAngleDeg) + 1);

        if(boardAngleDeg > 0)
        {
            newPizzaBoxBasePos.z = initpizzaBoxBasePos.y - newPizzaBoxBasePos.y;
        }
        else
        {
            newPizzaBoxBasePos.z = -(initpizzaBoxBasePos.y - newPizzaBoxBasePos.y);
        }
        pizzaBoxBase.localPosition = newPizzaBoxBasePos;
        

        //Fix world position issue with arm
        if(boardAngleDeg > 0)
        {
            LHandIK.position = Vector3.Lerp(initLHandIKPos, LHandTargetForward.position, Mathf.Abs(boardAngleDeg/30f));
        }
        else
        {
            LHandIK.position = Vector3.Lerp(initLHandIKPos, LHandTargetBackward.position, Mathf.Abs(boardAngleDeg/30f));
        }
    }
}
