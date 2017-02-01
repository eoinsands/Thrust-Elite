using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControl : MonoBehaviour {

	public GameObject target;
	public float gunRange = 20.0f;
	public float slewRate=1;

	enum AI {Searching, Tracking, Firing};
	Vector2 upperScan, lowerScan;
	float upperAngle, lowerAngle;
	AI turretAI;
	Guns gun;
	bool firing, targeting;

	// Use this for initialization
	void Start () {
		turretAI = AI.Searching;
		target = GameObject.FindGameObjectWithTag("Player");
		upperScan = new Vector2 (0, 16);
		upperAngle= Vector3.Angle (Vector2.up, upperScan);
		lowerScan = new Vector2 (16, -2);
		lowerAngle = Vector2.Angle (Vector2.up, lowerScan);
		gun=GetComponent<Guns>();
	}
	
	// Update is called once per frame
	void Update () {
		if (turretAI==AI.Searching){
			Searching();
		}
		if (targeting){
			if (!firing) {
				gun.StartFire();
				firing=true;
			} else {
				gun.FireWeapon(new Ray(transform.position, -transform.up));

			}
		}
		else {
			if (firing){
				gun.StopFire();
				firing=false;
			}
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
		targeting=false;
		// First check correct direction
		if (transform.position.x <=target.transform.position.x){
			
			// Then correct sector
			if (targetAngle>=upperAngle && targetAngle<=lowerAngle){
		
				// Finally correct range
				if (targetRange<=gunRange){
					targeting=true;
					Slew(180+targetAngle);
				}
			}
		}
		return AI.Searching;
	}

	void Slew(float targetAngle){
		transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (targetAngle, 90,0), Time.deltaTime);

	}
}
