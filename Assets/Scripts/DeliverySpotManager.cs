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
    private float elapsedTime = 0;

    private bool isPaused = false;

    void Awake()
    {
        deliverySpotList = new LinkedList<DeliverySpot>();
    }

    void OnEnable()
    {
        GameManager.pauseUpdate += UpdatePause;
    }

    void OnDisable()
    {
        GameManager.pauseUpdate -= UpdatePause;
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

    void Update()
    {
        if(isPaused)
        {
            CancelInvoke("ActivateNext");
        }
        else
            elapsedTime += Time.deltaTime;
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
        elapsedTime = 0;
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

    void UpdatePause(bool status)
    {
        isPaused = status;
        if (!isPaused)
        {
            Invoke("ActivateNext", spotChangeTime - elapsedTime);
        }
    }


}