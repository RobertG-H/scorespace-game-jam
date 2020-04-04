using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaBoxManager : MonoBehaviour
{
    struct PizzaBox
    {
        public PizzaBox(GameObject instance)
        {
            initPoint = instance.transform.localPosition;
            initRotation = instance.transform.localRotation;
            transform = instance.transform;
            rb = instance.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
        public Vector3 initPoint;
        public Quaternion initRotation;
        public Transform transform;
        public Rigidbody rb;
    }
    LinkedList<PizzaBox> pizzaBoxList;
    public GameObject pizzaBoxPrefab;
    public float flexiness;
    public float dropThresh;
    public float dropForce;
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
            AddPizzaBox();
        }
        var pizzaBox = pizzaBoxList.First;
        int idx = 0;
        while(pizzaBox != null)
        {
            Quaternion newRotation = Quaternion.identity;
            newRotation *= Quaternion.Euler(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
            pizzaBox.Value.transform.localRotation = pizzaBox.Value.initRotation * newRotation;
            
            Vector3 newPos = pizzaBox.Value.initPoint;
            newPos.y = (newPos.y - transform.position.y) * Mathf.Cos(transform.localEulerAngles.x * Mathf.Deg2Rad);
            // newPos.x = (newPos.y - transform.position.y) * (flexiness * idx) * Mathf.Sin(transform.localEulerAngles.z * Mathf.Deg2Rad);
            newPos.z = (newPos.y - transform.position.y) * (flexiness * idx) * Mathf.Sin(transform.localEulerAngles.x * Mathf.Deg2Rad);

            pizzaBox.Value.transform.localPosition = newPos;
            if(pizzaBox != pizzaBoxList.First && Mathf.Abs(pizzaBox.Value.transform.position.z - pizzaBox.Previous.Value.transform.position.z) > dropThresh)
            {
                pizzaBox.Value.rb.isKinematic = false;
                Debug.DrawRay(pizzaBox.Value.transform.position, pizzaBox.Value.transform.rotation * Vector3.up, Color.red, 1.0f);
                pizzaBox.Value.rb.AddForce(pizzaBox.Value.transform.rotation * Vector3.up * dropForce);
                var temp = pizzaBox;
                pizzaBox.Value.transform.parent = null;
                pizzaBoxList.Remove(pizzaBox);
                pizzaBox = temp.Next;   
            }
            else
            {
                pizzaBox = pizzaBox.Next;
            }
            idx += 1;
        }

    }

    
    public void AddPizzaBox()
    {
        GameObject pizzaBoxInstance = Instantiate(pizzaBoxPrefab, transform.position + new Vector3(0, pizzaBoxList.Count * (pizzaBoxHeight), 0), Quaternion.Euler(Vector3.up * Random.Range(-5, 5)));
        pizzaBoxInstance.transform.parent = transform;
        pizzaBoxList.AddLast(new PizzaBox(pizzaBoxInstance));
    }
}  
