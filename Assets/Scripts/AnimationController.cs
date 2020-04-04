using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    public Transform board;
    public Transform hips;
    public Transform head;
    public Transform leftHandIK;
    public Transform pizzaBoxBase;


    public Vector3 pizzaBoxOffset;
    public float leftHandIKMaxDist;
    public float pizzaBoxMaxDist;

    private float initHipsY;
    private float initLeftHandIKX;

    // Start is called before the first frame update
    void Start()
    {
        initHipsY = hips.position.y;
        initLeftHandIKX = leftHandIK.position.x;

    }

    // Update is called once per frame
    void Update()
    {
        hips.rotation = board.rotation;
        float boardAngle = board.localEulerAngles.x*Mathf.Deg2Rad;
        Vector3 newHipsPos = hips.position;
        newHipsPos.y = initHipsY * Mathf.Cos(boardAngle);
        hips.position = newHipsPos;

        Vector3 newPizzaBoxBasePos = hips.position + pizzaBoxOffset;
        newPizzaBoxBasePos.z = pizzaBoxMaxDist * Mathf.Sin(boardAngle);
        pizzaBoxBase.position = newPizzaBoxBasePos;
        
        Vector3 newleftHandIKPos = leftHandIK.position;
        newleftHandIKPos.z = leftHandIKMaxDist * Mathf.Sin(boardAngle);
        newleftHandIKPos.x = initLeftHandIKX * Mathf.Abs(Mathf.Cos(boardAngle));
        leftHandIK.position = newleftHandIKPos;
    }
}
