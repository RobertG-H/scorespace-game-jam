using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private float tilt = 5f;
    Transform target;

    public GameObject mesh;
    void OnEnable()
    {
        DeliverySpotManager.deliverySpotUpdate += OnDeliverySpotUpdate;
        PizzaBoxManager.pizzaBoxUpdate += OnPizzaUpdate;
    }

    void OnDisable()
    {
        DeliverySpotManager.deliverySpotUpdate -= OnDeliverySpotUpdate;
        PizzaBoxManager.pizzaBoxUpdate -= OnPizzaUpdate;

    }

    void Start()
    {
        mesh.SetActive(false);
    }

    void Update()
    {
        if (target) 
        {
            transform.LookAt(target);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + tilt, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }

    void OnDeliverySpotUpdate(Transform spot)
    {
        target = spot;
    }

    void OnPizzaUpdate(int count)
    {
        if (count == 0)
        {
            mesh.SetActive(false);
        }
        else
        {
            mesh.SetActive(true);
        }
    }
}
