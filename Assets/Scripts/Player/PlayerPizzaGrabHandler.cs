using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPizzaGrabHandler : MonoBehaviour
{

	public PizzaBoxManager pizzaBoxManager;
	public ScoreTextController scoreTextController;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Pizza Pickup")
		{
			int numPizzas = other.gameObject.GetComponent<PizzaPickupController>().numberOfPizzas;
			pizzaBoxManager.AddPizzaBox(numPizzas);
			Destroy(other.gameObject);
		}
		else if (other.gameObject.tag == "Delivery Spot")
		{
			int numPizzas = pizzaBoxManager.DeliverAll();
			GameManager.Instance.AddScore(numPizzas);
			scoreTextController.ShowScore(numPizzas);
			GameManager.Instance.pizzasDelivered += numPizzas;
		}
	}


}
