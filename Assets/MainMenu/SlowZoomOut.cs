using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowZoomOut : MonoBehaviour {

	RectTransform rect;
	Vector3 scale;
	public float speed=0.02f;

	// Use this for initialization
	void Start () {
		rect=GetComponent<RectTransform>();
		scale=rect.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		
		scale=Vector3.Lerp(scale, new Vector3 (1,1,1), speed*Time.deltaTime);

		rect.localScale=scale;
	}
}
