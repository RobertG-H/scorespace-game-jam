using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardLightController : MonoBehaviour
{
	public Gradient alphagrad;
	public Gradient textGrad;
	public Material glowmat;
	public Text speedtext;

	float lastFrameIntensity = 0f;

	public void SetGradLevel(float intensity, float speedfactor)
	{

		float thisFrameIntensity = Mathf.MoveTowards( lastFrameIntensity, Mathf.Max(-1f, intensity), 7f*Time.deltaTime);

		float evaluation = thisFrameIntensity * 0.5f + 0.5f;
		Color tosettext = textGrad.Evaluate(evaluation);
		Color toset = alphagrad.Evaluate(evaluation) * (100f * speedfactor);
		speedtext.color = tosettext;

		glowmat.SetColor("_EmissiveColor",toset );

		lastFrameIntensity = thisFrameIntensity;
	}
}
