using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControl : MonoBehaviour {

	public GameObject target;
	public float gunRange = 20.0f;

	enum AI {Searching, Tracking, Firing};
	Vector2 upperScan, lowerScan;
	float upperAngle, lowerAngle;
	AI turretAI;

	// Use this for initialization
	void Start () {
		turretAI = AI.Searching;
		target = GameObject.FindGameObjectWithTag("Player");
		upperScan = new Vector2 (0, 16);
		upperAngle= Vector3.Angle (Vector2.up, upperScan);
		lowerScan = new Vector2 (16, -2);
		lowerAngle = Vector2.Angle (Vector2.up, lowerScan);
	}
	
	// Update is called once per frame
	void Update () {
		if (turretAI==AI.Searching){
			Searching();
		}
	}

	AI Searching (){
		Vector2 targetRay=target.transform.position-transform.position;
		float targetAngle = Vector2.Angle (Vector2.up, targetRay);
		float targetRange = targetRay.magnitude;
		Debug.Log ("Searching");
		//Debug.Log ("Upper Angle: " + upperAngle + " Lower Angle: " + lowerAngle + " Target Angle: " + targetAngle + " Target Range: " + targetRange);
		Debug.DrawRay(transform.position, upperScan);
		Debug.DrawRay(transform.position, lowerScan);

		// First check correct direction
		if (transform.position.x <=target.transform.position.x){
			Debug.Log ("Pointing Right Direction");
			// Then correct sector
			if (targetAngle>=upperAngle && targetAngle<=lowerAngle){
				Debug.Log ("Correct Sector");
				// Finally correct range
				if (targetRange<=gunRange){
					Debug.Log ("Target Acquired");
					transform.LookAt(target.transform);
					transform.Rotate (new Vector3(-90,0,0));
				}
			}
		}
		return AI.Searching;
	}


}
