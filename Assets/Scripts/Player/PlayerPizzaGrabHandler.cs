using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPizzaGrabHandler : MonoBehaviour
{

	public PizzaBoxManager pizzaBoxManager;

	void OnTriggerEnter(Collider other)
	{
		Debug.Log("Pizza");
		if (other.gameObject.tag == "Pizza Pickup")
		{
			int numPizzas = other.gameObject.GetComponent<PizzaPickupController>().numberOfPizzas;
			pizzaBoxManager.AddPizzaBox(numPizzas);

			Destroy(other.gameObject);
		}
	}


}
