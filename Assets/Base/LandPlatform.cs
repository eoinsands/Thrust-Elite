using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandPlatform : MonoBehaviour {

	bool shopAccess=false;
	public GameObject Shop;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (shopAccess){
			Debug.Log ("Shop Access Available");
			if (Input.GetKeyDown(KeyCode.Return)){
				Shop.GetComponent<ShopFront>().OpenShopFront();
			}
		}
	}

	void OnCollisionStay(Collision collision){
		if (collision.collider.tag=="Player"){
			shopAccess=true;
		}
	}

	void OnCollisionExit(Collision collision){
		if (collision.collider.tag=="Player"){
			shopAccess=false;
		}
	}
}
