using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaBoxManager : MonoBehaviour
{
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
    public float boardRotationTracking;
    public float handRotationTracking;
    public float pizzaBoxDestroyDelay;
    private float pizzaBoxHeight;

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
        float baseXRot = transform.localEulerAngles.x;
        if(baseXRot > 180)
        {
            baseXRot -= 360;
        }
        Quaternion newBaseRotation = Quaternion.identity;
        float boardRotX = board.localEulerAngles.x;
        float handRotX = Mathf.Lerp(baseXRot, GetHandRotation(), handRotationTracking);

        newBaseRotation *= Quaternion.Euler(boardRotX + handRotX, board.localEulerAngles.y, 0);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, newBaseRotation, boardRotationTracking/(float)pizzaBoxList.Count);


        
        var pizzaBox = pizzaBoxList.First;
        int idx = 0;
        while(pizzaBox != null)
        {
            Quaternion newRotation = Quaternion.identity;
            newRotation *= Quaternion.Euler(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            pizzaBox.Value.transform.localRotation = pizzaBox.Value.initRotation * newRotation;
            
            Vector3 newPos = pizzaBox.Value.initPoint;
            newPos.y = (newPos.y) * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad);
            // newPos.x = (newPos.y - transform.position.y) * (flexiness * idx) * Mathf.Sin(transform.localEulerAngles.z * Mathf.Deg2Rad);
            newPos.z = (newPos.y) * (flexiness * idx) * Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad);

            pizzaBox.Value.transform.localPosition = newPos;
            
            float pizzaBoxXRot = pizzaBox.Value.transform.rotation.eulerAngles.x;
            if(pizzaBoxXRot > 180)
            {
                pizzaBoxXRot -= 360;
            }
            if(pizzaBox != pizzaBoxList.First && Mathf.Abs(pizzaBox.Value.transform.localPosition.z - pizzaBox.Previous.Value.transform.localPosition.z) > dropThreshDist || Mathf.Abs(pizzaBoxXRot) > dropThresAngle)
            {
                pizzaBox.Value.rb.isKinematic = false;
                Debug.DrawRay(pizzaBox.Value.transform.position, pizzaBox.Value.transform.rotation * Vector3.up, Color.red, 1.0f);
                pizzaBox.Value.rb.AddForce(pizzaBox.Value.transform.rotation * Vector3.up * dropForce);
                var temp = pizzaBox;
                pizzaBox.Value.transform.parent = null;
                pizzaBoxList.Remove(pizzaBox);
                Destroy(pizzaBox.Value.instance, pizzaBoxDestroyDelay);
                pizzaBox = temp.Next;   
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
    }

    public float GetHandRotation()
    {
        //Linear
        // return 18*((playerController.mousePos.y) - (Screen.height/2)/Screen.dpi);

        if((playerController.mousePos.x) >= 0)
        {
            // return -18*(playerController.mousePos.y);
            return -Mathf.Atan2(playerController.mousePos.y*Mathf.Abs(Mathf.Sin(playerController.mousePos.x)), Screen.width/2/Screen.dpi) * Mathf.Rad2Deg;

        }
        else
        {
            // return 18*(playerController.mousePos.y);
            return Mathf.Atan2(playerController.mousePos.y*Mathf.Abs(Mathf.Sin(playerController.mousePos.x)), Screen.width/2/Screen.dpi) * Mathf.Rad2Deg;
        }
    }
}  


