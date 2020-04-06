using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverySpotManager : MonoBehaviour
{

    public delegate void OnDeliverySpotUpdate(Transform position);
    public static event OnDeliverySpotUpdate deliverySpotUpdate;
    struct DeliverySpot
    {
        public DeliverySpot(GameObject instance)
        {
            transform = instance.transform;
            this.instance = instance;
        }

        public Transform transform;
        public GameObject instance;
    }

    LinkedList<DeliverySpot> deliverySpotList;
    LinkedListNode<DeliverySpot> activeSpot;

    public GameObject deliverySpotPrefab;
    public List<Transform> deliverySpotLocations;
    public float spotChangeTime;


    void Awake()
    {
        deliverySpotList = new LinkedList<DeliverySpot>();
    }

    void Start()
    {
        foreach(Transform trans in deliverySpotLocations)
        {
            GameObject deliverySpotInstance = Instantiate(deliverySpotPrefab, trans.position, trans.rotation);
            deliverySpotList.AddLast(new DeliverySpot(deliverySpotInstance));
        }
        DeactiveAll();
        activeSpot = deliverySpotList.First;
        activeSpot.Value.instance.SetActive(true);
        deliverySpotUpdate(activeSpot.Value.instance.transform);
        Invoke("ActivateNext", spotChangeTime);
    }

    void ActivateNext()
    {
        activeSpot.Value.instance.SetActive(false);
        LinkedListNode<DeliverySpot> nextSpot = activeSpot.Next;
        if (nextSpot == null)
        {
            nextSpot = deliverySpotList.First;
        }
        activeSpot = nextSpot;
        activeSpot.Value.instance.SetActive(true);
        deliverySpotUpdate(activeSpot.Value.instance.transform);
        Invoke("ActivateNext", spotChangeTime);
    }

    void DeactiveAll()
    {
        foreach(DeliverySpot ds in deliverySpotList)
        {
            ds.instance.SetActive(false);
        }
    }


}