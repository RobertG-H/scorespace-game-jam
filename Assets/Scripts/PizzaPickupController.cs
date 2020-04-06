using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaPickupController : MonoBehaviour
{
    public Transform hoverTarget1;
    public Transform hoverTarget2;
    public GameObject firstPizzaBox;
    public GameObject pizzaBoxPrefab;
    public float rotationSpeed;
    public float hoverSpeed;
    public int numberOfPizzas;
    
    // Start is called before the first frame update
    void Start()
    {        
        float pizzaBoxHeight = 0.14f;//firstPizzaBox.transform.lossyScale.y;
        for(int i = 0; i < numberOfPizzas-1; i++)
        {
            GameObject pizzaBoxInstance = Instantiate(pizzaBoxPrefab, firstPizzaBox.transform.position + new Vector3(0, (i+1) * (pizzaBoxHeight), 0), Quaternion.Euler(Vector3.up * Random.Range(-5, 5)));
            pizzaBoxInstance.transform.parent = firstPizzaBox.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        firstPizzaBox.transform.Rotate(0, rotationSpeed* Time.deltaTime, 0);
        firstPizzaBox.transform.position = Vector3.Lerp(hoverTarget1.position, hoverTarget2.position, Hermite(Mathf.PingPong(Time.time * hoverSpeed, 1)));
    }

    private float Hermite(float t)
    {
        return -t*t*t*2f + t*t*3f;
    }
}
