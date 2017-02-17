using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour {

	public float thrust=1f;
	public float maxSpeed=10f;
	public float aggroDist = 15;
	public float rotateSpeed = 4.5f;
	public float maxHealth=100f;
	public Text text;

	GameObject target;
	ParticleSystem thruster;
	AudioSource audiosource;
	Rigidbody rigidbody;
	IWeapon weapon;
	Shield shield;

	enum AIState {Idle, Launching, Tracking, Firing};
	AIState currentState = AIState.Idle;
	AIState previousState = AIState.Idle;

	float health;
	float startTime;
	float rotateInput;
	bool thrusting;
	bool firing;

	// Use this for initialization 
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		target = GameObject.FindGameObjectWithTag("Player");
		thruster=GetComponentsInChildren<ParticleSystem>()[0];
		audiosource=GetComponent<AudioSource>();
		shield=GetComponentInChildren<Shield>();
		weapon = GetComponentInChildren<IWeapon>();

		health=maxHealth;
		thruster.Stop();
	}
	
	// Update is called once per frame
	void Update () {

		// desiredVel is normalised vector from object to target
		Vector3 objectToTarget = target.transform.position - transform.position;

		transform.Rotate (Vector3.up*rotateInput*rotateSpeed);
		CheckFire(objectToTarget);

		// STATE MACHINE BEHAVIOR
		if (currentState==AIState.Idle){
			IdleBehaviour(objectToTarget.magnitude);
		}

		if (currentState==AIState.Launching){
			Launching();
		}

		if (currentState==AIState.Tracking){
			TrackingBehaviour(objectToTarget);

		}

		text.text = currentState.ToString();

		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
	}

	void IdleBehaviour (float distance){
		if (distance<aggroDist){
			currentState=AIState.Launching;
			startTime=Time.time;
		}
	}

	void Launching(){
		if (Time.time<startTime+1.0f){
			FireThrusters();
		}
		else {
			currentState=AIState.Tracking;
		}
	}

	void TrackingBehaviour(Vector3 objectToTarget){
		
		// If target is greater than 2 degrees from nose
		if (Vector2.Angle(objectToTarget, transform.forward)>2f){
			// Turn left or right as required
			if (AngleDir2(objectToTarget, transform.forward)>0){
				rotateInput=-1;
			} else if (AngleDir2(objectToTarget, transform.forward)<0){
				rotateInput=1;
			}
		}

		Debug.DrawRay (transform.position, objectToTarget, Color.blue);

		// If target is within 45 degrees of nose fire thrusters, else don't
		if (Vector2.Angle(objectToTarget, transform.forward)<45f){
			FireThrusters();
		} else {
			StopThrusters();
		}
	}


	void FireThrusters(){
		
		thruster.Play();
		if (!thrusting){
			audiosource.Play();
			thrusting=true;
		}

		rigidbody.AddForce(transform.forward*thrust);
		//Once you start thrusting, antigrav is disabled
		//rigidbody.useGravity = true;
	}


	void CheckFire(Vector3 objectToTarget){
		// If target is on the nose and within range
		if ((Vector2.Angle(objectToTarget, transform.forward)<10f)&&(objectToTarget.magnitude<weapon.Range())){
			//  If object can see target
			RaycastHit hitInfo;
			Physics.Raycast(transform.position, Vector3.Normalize(objectToTarget), out hitInfo);
			if (hitInfo.transform==target.transform){

				if (!firing){
					weapon.StartFire();
					firing=true;
				} else {
					weapon.FireWeapon(new Ray(transform.position, transform.forward));
				}
			}
		} else {
			weapon.StopFire();
			firing=false;
		}
	}

	void StopThrusters(){
		audiosource.Stop();
		thruster.Stop();
		thrusting=false;
	}

	public void TakeFire(float damage, RaycastHit hitInfo){
		
		health-=damage;
		shield.lastHit = Time.time;
		if (health<=0){
			Destroy(gameObject);
		}
	}

	float AngleDir (Vector3 fwd, Vector3 targetDir, Vector3 up){
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);

		if (dir>0f){
			return 1f;
		} else if (dir <0f){
			return -1f;
		} else {
			return 0f;
		}
	}

	float AngleDir2 (Vector2 A, Vector2 B){
		return -A.x * B.y + A.y * B.x;
	}

	void TrackingBehaviour2(){
		
		Rigidbody targetRigid = target.GetComponent<Rigidbody>();
		//Vector3 aimPoint = target.transform.position - Vector3.Normalize(targetRigid.velocity)*3;
		Vector3 aimPoint = target.transform.position;// - targetRigid.velocity;
		Debug.DrawLine (transform.position, aimPoint);

		Vector3 desiredVel = Vector3.Normalize(aimPoint - transform.position)*1;
		Vector3 steeringForce = desiredVel - rigidbody.velocity;


		Debug.DrawRay (transform.position, desiredVel, Color.blue);
		Debug.DrawRay (transform.position, steeringForce, Color.red);
		FireThrusters();
		if (AngleDir2 (steeringForce, transform.forward)<-0){
			rotateInput = 1;
			//StopThrusters();
			Debug.Log ("Right Turn");
		}	else if (AngleDir2 (desiredVel, transform.forward)>0){
			rotateInput = -1;
			//StopThrusters();
			Debug.Log ("Left Turn");
		} else {
			
		}
		Debug.Log (AngleDir2 (steeringForce, transform.forward));
	}
}
