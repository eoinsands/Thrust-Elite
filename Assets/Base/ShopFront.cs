using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopFront : MonoBehaviour {

	public GameObject portMenu;

	// Use this for initialization
	void Start () {
		portMenu=GameObject.FindGameObjectWithTag("Menu");
		Debug.Log (portMenu.name);
		//portMenu.SetActive(false);
		Debug.Log (portMenu.name);	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (portMenu.name);	
	}

	public void OpenShopFront(){
		//portMenu.SetActive(true);
		Debug.Log("Open For Business!!");


	}

}
