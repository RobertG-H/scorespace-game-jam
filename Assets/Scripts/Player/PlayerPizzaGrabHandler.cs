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
			int score = numPizzas;
			if (numPizzas > 20)
				score += 20;
			else if (numPizzas > 15)
				score += 15;
			else if (numPizzas > 10)
				score += 10;
			else if (numPizzas > 5)
				score += 5;
			PlayerController player = GetComponent<PlayerController>();
			score += (int) (8 * ((player.speed * 0.05f) * (player.speed * 0.05f)));
			GameManager.Instance.AddScore(score);
			scoreTextController.ShowScore(score);
			GameManager.Instance.AddPizzaCount(numPizzas);
		}
	}


}
