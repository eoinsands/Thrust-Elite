using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,5);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
