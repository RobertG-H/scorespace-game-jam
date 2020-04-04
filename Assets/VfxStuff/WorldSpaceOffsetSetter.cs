using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class WorldSpaceOffsetSetter : MonoBehaviour
{
	Vector3 lastFramePos = Vector3.zero;
	VisualEffect vfx;
	PlayerController pcont;

	private void Start()
	{
		lastFramePos = this.transform.position;
		vfx = this.gameObject.GetComponent<VisualEffect>();
		pcont = Object.FindObjectOfType<PlayerController>();
	}

	// Update is called once per frame
	void Update()
    {
		Vector3 wsOffset = this.transform.position - lastFramePos;

		vfx.SetVector3("WsOffset", wsOffset);
		vfx.SetVector3("EmitterOffset", this.transform.position);
		vfx.SetFloat("RawSpeed", pcont.velocity);

		lastFramePos = this.transform.position;
    }
}
