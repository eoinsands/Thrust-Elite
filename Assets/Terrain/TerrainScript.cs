using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour, IDestructable {


	public float health=0.01f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TakeFire(float damage, RaycastHit hitInfo){
		health-=damage;
		if (health<=0){
			Destroy(gameObject);
		}
	}

}
