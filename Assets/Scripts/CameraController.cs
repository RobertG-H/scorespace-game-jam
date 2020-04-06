using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
    Transform playerTransfrom;
	PlayerController pcont;

	[SerializeField]
	float xOffsetMax = 5f;

	[SerializeField]
	float distance = 17f;

	public float distanceMult = 1f;

    Vector3 offsetBaseDirection;
	float additionalY = 0f;

	float xOffset =0f;
	

	float lastFramePlayerYangle;

    // Start is called before the first frame update
    void Start()
    {
        offsetBaseDirection = transform.localPosition.normalized;
		lastFramePlayerYangle = playerTransfrom.eulerAngles.y;
		pcont= Object.FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
		if(pcont.isPaused) return;
		HandleAngleLag();
		HandleSpeedDip();
		Vector3 newLocal = offsetBaseDirection*distance*distanceMult;
		newLocal.y += additionalY;
		newLocal.x = xOffset;
		this.transform.localPosition = newLocal;
	}

	float modulo(float A, float B)
	{
		float ret = A % B;

		if (ret < 0)
			ret += B;

		return ret;
	}


	void HandleAngleLag()
	{
		float turnAmount = Input.mousePosition.x / Screen.width;
		turnAmount = turnAmount * 2f - 1f;
		xOffset = Mathf.Lerp(xOffset, xOffsetMax * turnAmount, 0.2f);
	}


	void HandleSpeedDip()
	{
		float speedDipMult =Mathf.Clamp01(pcont.speed*0.005f);
		speedDipMult *= speedDipMult*speedDipMult;

		distanceMult = 1f + speedDipMult * 0.3f;

		additionalY = -speedDipMult * 4f;

	}


}
