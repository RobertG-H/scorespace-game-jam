﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFovController : MonoBehaviour
{

	[SerializeField]
	Vector2 minMaxSpeedRangeToAdjustFov = new Vector2(5, 100);
	[SerializeField]
	Vector2 minMaxFovFromSpeed = new Vector2(40, 90);
	[SerializeField]
	float lerpSpeed=1f;

	[SerializeField]
	PlayerController playerController = null;
	Camera cam;

	// Start is called before the first frame update
	void Start()
    {
		cam = this.gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
		if (playerController.isPaused) return;
		HandleFovTransform();
	}


	void HandleFovTransform()
	{
		float fov = Mathf.Clamp01((playerController.speed - minMaxSpeedRangeToAdjustFov.x) / (minMaxSpeedRangeToAdjustFov.y - minMaxSpeedRangeToAdjustFov.x));
		fov = fov * (minMaxFovFromSpeed.y - minMaxFovFromSpeed.x) + minMaxFovFromSpeed.x;

		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, lerpSpeed);
	}

	

}
