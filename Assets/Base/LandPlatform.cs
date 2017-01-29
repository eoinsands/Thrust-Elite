using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandPlatform : MonoBehaviour {

	bool shopAccess=false;
	//public GameObject Shop;
	public GameObject portMenu;

	// Use this for initialization
	void Start () {
		portMenu=GameObject.FindGameObjectWithTag("Menu");
		portMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (shopAccess){
			//Debug.Log ("Shop Access Available");
			if (Input.GetKeyDown(KeyCode.Return)){
				//Shop.GetComponent<ShopFront>().OpenShopFront();
				OpenMenu();
			}
		}
	}

	void OpenMenu(){
		portMenu.SetActive(true);
		portMenu.GetComponent<MenuControl>().fadeIn=true;
		GameObject.FindObjectOfType<PlayerControl>().inMenu=true;
	}

	public void CloseMenu(){
		portMenu.GetComponent<MenuControl>().fadeOut=true;
		GameObject.FindObjectOfType<PlayerControl>().inMenu=false;
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
