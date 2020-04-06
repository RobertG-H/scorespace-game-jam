using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaBoxManager : MonoBehaviour
{
    public delegate void OnPizzaBoxListUpdate(int count);
    public static event OnPizzaBoxListUpdate pizzaBoxUpdate;
    struct PizzaBox
    {
        public PizzaBox(GameObject instance, Quaternion initRotation)
        {
            initPoint = instance.transform.localPosition;
            this.initRotation = initRotation;
            transform = instance.transform;
            rb = instance.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            this.instance = instance;
        }
        public Vector3 initPoint;
        public Quaternion initRotation;
        public Transform transform;
        public Rigidbody rb;
        public GameObject instance;
    }
    LinkedList<PizzaBox> pizzaBoxList;
    public GameObject pizzaBoxPrefab;
    public Transform board;
    public PlayerController playerController;

    public float flexiness;
    public float dropThreshDist;
    public float dropThresAngle;
    public float dropForce;
    public float rollTracking;
    public float rollAngleFlex;
    public float rollDistFlex;
    public float handRotationTracking;
    public float boardRotationTracking;
    public float pizzaBoxDestroyDelay;
    private float pizzaBoxHeight;

    private float rollModifier = 0;

    // Start is called before the first frame update
    void Start()
    {         
        pizzaBoxHeight = pizzaBoxPrefab.transform.lossyScale.y;
        pizzaBoxList = new LinkedList<PizzaBox>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("a"))
        {
            AddPizzaBox(1);
        }


        // float baseXRot = transform.localEulerAngles.x;
        float rollAngleDelta = playerController.rollAngleDelta;
        rollModifier = Mathf.Lerp(rollModifier, rollAngleDelta, rollTracking);
        // rollModifier = 0;


        // float handRotX = GetHandRotation()*Time.deltaTime*handRotationTracking/Mathf.Max(1, (float)pizzaBoxList.Count) + baseXRot;
        // handRotX = Mathf.Clamp(handRotX, -60, 60);
        Quaternion newBaseRotation = Quaternion.identity;
        newBaseRotation *= Quaternion.Euler(board.localEulerAngles.x, board.localEulerAngles.y, 0);//remove boardrot

        transform.localRotation = Quaternion.Slerp(transform.localRotation, newBaseRotation, boardRotationTracking/Mathf.Max(1, (float)pizzaBoxList.Count));
        
        float handRotX = GetHandRotation();
        newBaseRotation = Quaternion.identity;
        newBaseRotation *= Quaternion.Euler(handRotX, 0, 0);//remove boardrot
        transform.localRotation = Quaternion.Slerp(transform.localRotation, newBaseRotation, handRotationTracking/Mathf.Max(1, (float)pizzaBoxList.Count));


        
        var pizzaBox = pizzaBoxList.First;
        int idx = 0;
        while(pizzaBox != null)
        {
            Quaternion newRotation = Quaternion.identity;
            newRotation *= Quaternion.Euler(transform.eulerAngles.x - rollModifier*rollAngleFlex, 0, transform.eulerAngles.z);
            pizzaBox.Value.transform.localRotation = pizzaBox.Value.initRotation * newRotation;
            
            Vector3 newPos = pizzaBox.Value.initPoint;
            newPos.y = (newPos.y) * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad);
            // newPos.x = (newPos.y - transform.position.y) * (flexiness * idx) * Mathf.Sin(transform.localEulerAngles.z * Mathf.Deg2Rad);
            newPos.z = (newPos.y) * (flexiness * idx) * Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad) - (rollModifier*idx*rollDistFlex);

            pizzaBox.Value.transform.localPosition = newPos;
            
            float pizzaBoxXRot = ConvertTo180(pizzaBox.Value.transform.rotation.eulerAngles.x);
 
            if(pizzaBox != pizzaBoxList.First && pizzaBox.Next == null && Mathf.Abs(pizzaBox.Value.transform.localPosition.z - pizzaBox.Previous.Value.transform.localPosition.z) > dropThreshDist || Mathf.Abs(pizzaBoxXRot) > dropThresAngle)
            {
                pizzaBox.Value.rb.isKinematic = false;
                Debug.DrawRay(pizzaBox.Value.transform.position, pizzaBox.Value.transform.rotation * Vector3.up, Color.red, 1.0f);
                pizzaBox.Value.rb.AddForce(pizzaBox.Value.transform.rotation * Vector3.up * dropForce);
                var temp = pizzaBox;
                pizzaBox.Value.transform.parent = null;
                pizzaBoxList.Remove(pizzaBox);
                Destroy(pizzaBox.Value.instance, pizzaBoxDestroyDelay);
                pizzaBox = temp.Next;   
                pizzaBoxUpdate(pizzaBoxList.Count);
            }
            else
            {
                pizzaBox = pizzaBox.Next;
            }
            idx += 1;
        }

    }

    
    public void AddPizzaBox(int numPizzas)
    {
        for(int i = 0; i < numPizzas; i++)
        {
            Quaternion initRotation = Quaternion.Euler(Vector3.up * Random.Range(-5, 5));
            GameObject pizzaBoxInstance = Instantiate(pizzaBoxPrefab, transform.position + new Vector3(0, pizzaBoxList.Count * (pizzaBoxHeight), 0), initRotation);
            pizzaBoxInstance.transform.parent = transform;
            pizzaBoxList.AddLast(new PizzaBox(pizzaBoxInstance, initRotation));
        }
        pizzaBoxUpdate(pizzaBoxList.Count);
    }
    public int DeliverAll()
    {
        if (pizzaBoxList.Count == 0) return 0;
        int numPizzas = pizzaBoxList.Count;
        foreach (PizzaBox box in pizzaBoxList)
        {
            Destroy(box.instance);
        }
        pizzaBoxList = new LinkedList<PizzaBox>();
        return numPizzas;
    }

    public float GetHandRotation()
    {
		//Linear
		// return 18*((playerController.mousePos.y) - (Screen.height/2)/Screen.dpi);

		float handHeight = playerController.mousePos.y / (Screen.height*0.5f / Screen.dpi);

		handHeight *= playerController.mousePos.x / (Screen.width*0.5f / Screen.dpi);

		return -handHeight*50f;
    }

    private float ConvertTo180(float angle)
    {
        //Convert angle from 0 to 360 to be between -180 and 180
        if(angle > 180)
        {
            return angle -360;
        }
        return angle;
    }
}  


