using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAnimation : MonoBehaviour {

	public float iSpeed=5f;

	Material mMaterial;
	float mTime;

	// Use this for initialization
	void Start () {
		mMaterial = GetComponent<Renderer>().material;
		mTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		mTime += Time.deltaTime*iSpeed;
		mTime = Mathf.Repeat (mTime, 1.0f);
		mMaterial.SetFloat ("_Offset", mTime);
	}
}
