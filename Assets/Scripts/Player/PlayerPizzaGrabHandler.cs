﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPizzaGrabHandler : MonoBehaviour
{

	public PizzaBoxManager pizzaBoxManager;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Pizza Pickup")
		{
			int numPizzas = other.gameObject.GetComponent<PizzaPickupController>().numberOfPizzas;
			pizzaBoxManager.AddPizzaBox(numPizzas);
		}
		else if (other.gameObject.tag == "Delivery Spot")
		{
			GameManager.Instance.AddScore( pizzaBoxManager.DeliverAll());
		}
	}


}
