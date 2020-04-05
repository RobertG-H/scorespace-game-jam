using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
    Transform playerTransfrom;
	[SerializeField]
	float xRot = 21f;

	[SerializeField]
	float xOffsetMax = 5f;
	float yAngleMax = 10f;

	[SerializeField]
	float distance = 17f;

	public float distanceMult = 1f;

    Vector3 offsetBaseDirection;

	float xOffset =0f;
	

	float lastFramePlayerYangle;

    // Start is called before the first frame update
    void Start()
    {
        offsetBaseDirection = transform.localPosition.normalized;
		lastFramePlayerYangle = playerTransfrom.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
		HandleAngleLag();
		Vector3 newLocal = offsetBaseDirection*distance*distanceMult;
		newLocal.x = xOffset;
		this.transform.localPosition = newLocal;
		// transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
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


}
