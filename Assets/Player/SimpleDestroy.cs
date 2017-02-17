using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDestroy : MonoBehaviour {

	public void Destroy(){
		Debug.Log ("Destroyed");
		Destroy (gameObject);

	}
}
