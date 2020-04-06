using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    
    Transform target;
    void OnEnable()
    {
        DeliverySpotManager.deliverySpotUpdate += OnDeliverySpotUpdate;
    }

    void OnDisable()
    {
        DeliverySpotManager.deliverySpotUpdate -= OnDeliverySpotUpdate;
    }

    void Start()
    {
    }

    void Update()
    {
        if (target) 
        {
            transform.LookAt(target);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - 90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }

    void OnDeliverySpotUpdate(Transform spot)
    {
        Debug.Log("Updated");
        target = spot;
    }
}
