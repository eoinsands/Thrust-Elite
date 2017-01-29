using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour, IDestructable {

	public float health=10.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TakeFire(float damage){
		health-=damage;
		if (health<=0){
			Destroy(gameObject);
		}
	}
}
