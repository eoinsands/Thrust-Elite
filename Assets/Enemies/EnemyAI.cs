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
	public float avoidRange=2f;
	public Text text;
	public Text text2;
	public Text text3;

	GameObject target;
	ParticleSystem thruster;
	AudioSource audiosource;
	Rigidbody rigidbody;
	IWeapon weapon;
	Shield shield;

	enum AIState {Idle, Launching, Tracking, Avoiding, Manouevring, Patrolling, Returning, Seeking, Landing};
	enum AvoidState {Null, Left, Right, AboutTurn};
	AIState currentState = AIState.Idle;
	AIState previousState = AIState.Idle;
	AvoidState avoidState = AvoidState.Null;

	float health;
	float startTime;
	float breakOutTime;
	float breakOutAngle;
	float rotateInput;
	float targetLostTime;
	bool thrusting;
	bool firing;
	bool avoidOverride;
	int breakOutFactor=5;
	Vector3 startLocation;
	Vector2 lastKnownPosition;
	public List<Vector2> breadCrumbs = new List<Vector2>();

	// Use this for initialization 
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		target = GameObject.FindGameObjectWithTag("Player");
		thruster=GetComponentsInChildren<ParticleSystem>()[0];
		audiosource=GetComponent<AudioSource>();
		shield=GetComponentInChildren<Shield>();
		weapon = GetComponentInChildren<IWeapon>();
		startLocation=transform.position;
		breadCrumbs.Add(startLocation+new Vector3 (0,2,0));
		health=maxHealth;
		thruster.Stop();
	}
	
	// Update is called once per frame
	void Update () {
		avoidRange=rigidbody.velocity.magnitude/2;
		// desiredVel is normalised vector from object to target
		Vector2 objectToTarget = target.transform.position - transform.position;

		transform.Rotate (Vector3.up*rotateInput*rotateSpeed);

		CheckFire(objectToTarget);
		if (!avoidOverride){
			TerrainAvoid();
		}

		if (currentState!=AIState.Returning){
			BreadCrumbTrail();
		}
		WatchingOut(objectToTarget);

		// STATE MACHINE BEHAVIOR
		if (currentState==AIState.Idle){
			IdleBehaviour(objectToTarget);
		}

		if (currentState==AIState.Launching){
			Launching();
		}

		if (currentState==AIState.Tracking){
			TrackingBehaviour(objectToTarget);
		}

		if (currentState==AIState.Manouevring){
			ManouevringBehaviour(objectToTarget);
		}

		if (currentState==AIState.Patrolling){
			PatrollingBehaviour(objectToTarget);
		}

		if (currentState==AIState.Seeking){
			SeekingBehaviour(objectToTarget);
		}

		if (currentState==AIState.Returning){
			ReturningBehaviour();
		}

		if (currentState==AIState.Landing){
			LandingBehaviour();
		}

		text.text = currentState.ToString();
		text2.text = avoidState.ToString();
		text3.text = breadCrumbs.Count.ToString();

		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
	}

	void IdleBehaviour (Vector2 objectToTarget){
	// If within range
		if (objectToTarget.magnitude<aggroDist){

			// If visible
			if (IsVisible(objectToTarget)){
				currentState=AIState.Launching;
				startTime=Time.time;
			}
		}
	}

	void PatrollingBehaviour(Vector2 objectToTarget){
		//For now, just return to Tracking State
		startTime=Time.time;
		currentState=AIState.Tracking;
	}

	bool IsVisible(Vector2 objectToTarget){

		RaycastHit hitInfo;
		Physics.Raycast(transform.position, Vector3.Normalize(objectToTarget), out hitInfo);
		if (hitInfo.transform==target.transform){
			return true;
		}
		return false;

	}

	void WatchingOut(Vector2 objectToTarget){
		// At any point, if can see target, track (unless idle or launching)
		if (IsVisible(objectToTarget)&&currentState!=AIState.Idle&&currentState!=AIState.Landing&&objectToTarget.magnitude<aggroDist*3){
			currentState=AIState.Tracking;
		}
	}

	void Launching(){
		if (Time.time<startTime+1.0f){
			FireThrusters();
		}
		else {
			startTime=Time.time;
			currentState=AIState.Tracking;
		}
	}

	void TrackingBehaviour(Vector2 objectToTarget){
		avoidOverride=false;
		// If not still in sight, switch to seek
		if (!IsVisible(objectToTarget)){
			lastKnownPosition=target.transform.position;
			currentState = AIState.Seeking;
		}
		
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

		if (Time.time>startTime+5.0f){
			currentState=AIState.Manouevring;
			startTime=Time.time;
		}
	}

	bool Nav2Tgt (Vector2 navTarget){
		// Flies object to a given target and returns true if within 0.2 clicks of it

		Vector2 objectToTarget = navTarget - (Vector2)transform.position;
		Debug.DrawRay(transform.position, objectToTarget);
		//Debug.Log ("Navving");
		if (objectToTarget.magnitude<=2f){
			return true;
		}

		if (Vector2.Angle(objectToTarget, transform.forward)>2f){
			// Turn left or right as required
			if (AngleDir2(objectToTarget, transform.forward)>0){
				rotateInput=-1;
			} else if (AngleDir2(objectToTarget, transform.forward)<0){
				rotateInput=1;
			}
		}
		// Cheat by slowing down as approaching nav point
		if (objectToTarget.magnitude<=maxSpeed){
			rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, objectToTarget.magnitude);
		}
		FireThrusters();
		return false;
	}

	void ManouevringBehaviour(Vector2 objectToTarget){
		// Currently just travelling in a straight line to break things up, but only if above ground
		if (transform.position.y<0){
			currentState=AIState.Tracking;
			return;
		}
		rotateInput=0;
		FireThrusters();
		if (Time.time>startTime+3.0f){
			startTime=Time.time;
			if (IsVisible(objectToTarget)){
				currentState=AIState.Tracking;
			} else {
				currentState=AIState.Patrolling;
			}
		}
	}

	void SeekingBehaviour(Vector2 objectToTarget){
		// Go to last known position
		if (Nav2Tgt (lastKnownPosition)) {
			// If there and can't see target, return to base
			currentState=AIState.Returning;
		}
	}		

	void ReturningBehaviour(){
		//avoidOverride=true;
		Vector2 activeCrumb=breadCrumbs[breadCrumbs.Count-1];
		//Debug.Log (activeCrumb.ToString());
		// Move to last crumb
		//Debug.Log ("Breadcrumbs in list" + breadCrumbs.Count);
		Nav2Tgt(activeCrumb);
		if (Nav2Tgt(activeCrumb)){
			// Once there, if it's the last crumb then land
			if (breadCrumbs.Count==1){
				currentState=AIState.Landing;
			} else {
				// Otherwise remove that crumb and continue
				breadCrumbs.RemoveAt(breadCrumbs.Count-1);
			}
		}
	}

	void LandingBehaviour(){

		StopThrusters();
		if (AngleDir2(Vector2.up, transform.forward)>0){
				rotateInput=-1;
			} else if (AngleDir2(Vector2.up, transform.forward)<0){
				rotateInput=1;
			}
		if (Vector2.Angle(Vector3.up, transform.forward)<3f){
			currentState=AIState.Idle;
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
		rigidbody.useGravity = true;
	}

	void CheckFire(Vector3 objectToTarget){
		// If target is on the nose and within range
		if ((Vector2.Angle(objectToTarget, transform.forward)<10f)&&(objectToTarget.magnitude<weapon.Range())){
			//  If object can see target
			if (IsVisible(objectToTarget)){

				if (!firing){
					weapon.StartFire();
					firing=true;
				} else {
					weapon.FireWeapon(new Ray(transform.position, transform.forward));
				}
			}
		} else if (firing) {
			weapon.StopFire();
			firing=false;
		}
	}

	void TerrainAvoid(){
		//If not already avoiding
		int layermask = 1 << 8;
		layermask= ~layermask;

		if (currentState!=AIState.Avoiding){
			// Look forward as far as avoidRange - if something other than the target is in the way, avoid
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.forward, out hit, avoidRange, layermask)){
				//Set state to Avoiding and remember previous state
				previousState = currentState;
				currentState = AIState.Avoiding;
				avoidState = FindClearDirection();
				//Debug.Break();
			}
		}else {
			// If we are in avoid mode and not already breaking out, check twice as far out and set breakout time if clear
			RaycastHit hit;
			if (!Physics.Raycast(transform.position, transform.forward, out hit, avoidRange*breakOutFactor, layermask)&&breakOutTime==0){
				breakOutTime=Time.time;

			}

			// If we are in breakout mode, breakOutTime will be valid otherwise it will be 0
			// If longer than a second since breakout activated, go back to previous mode
			if ((breakOutTime!=0)&&(Time.time-breakOutTime>=breakOutAngle/75)){
				currentState=previousState;
				avoidState=AvoidState.Null;
				breakOutTime=0;
			} else if ((breakOutTime!=0)&&(Time.time-breakOutTime>=breakOutAngle/100)){
				FireThrusters();
			} else {
				if (avoidState == AvoidState.Left){
					rotateInput=-1;
					FireThrusters();
				}
				if (avoidState == AvoidState.Right){
					rotateInput=1;
					FireThrusters();
				}
				if (avoidState == AvoidState.AboutTurn){
					if (thrusting){
						StopThrusters();
					}
					rigidbody.useGravity=false;
					rotateInput=1;
				}
			}
		}
	}

	void BreadCrumbTrail(){
		int layermask = 1 << 8;
		layermask= ~layermask;

		Vector2 lastCrumb=breadCrumbs[breadCrumbs.Count-1];
		// If the last breadcrumb isn't visible from current location, first check if you can see any earlier crumbs
		Vector2 crumbRay=lastCrumb-(Vector2)transform.position;
		int crumbnum=0;
		foreach (Vector2 crumb in breadCrumbs){
			crumbRay=crumb-(Vector2)transform.position;
			if (!Physics.Raycast(transform.position, crumbRay, crumbRay.magnitude, layermask)){
				// If so, remove all breadcrumbs past the earliest visible one
				breadCrumbs.RemoveRange (crumbnum+1, breadCrumbs.Count-(crumbnum+1));
				break;
			}
			crumbnum++;
		}

		// Check if anything is in the way between object and last crumb
		if (Physics.Raycast(transform.position, crumbRay, crumbRay.magnitude, layermask)){

			//Debug.Log ("Lost Sight");

			// If not, create a new crumb at current location
//			if (crumbnum==breadCrumbs.Count){
				breadCrumbs.Add(transform.position);
//			}
		}
	}

	AvoidState FindClearDirection(){
		for (float scanAngle=15; scanAngle<=90; scanAngle+=15){
			RaycastHit hit;
			// Check twice scan distance out to the left
			if (!Physics.Raycast (transform.position, (Quaternion.Euler(0, 0, scanAngle)*transform.forward), out hit, avoidRange*breakOutFactor)){
				breakOutAngle=scanAngle;
				return AvoidState.Left;
			}
			// and right
			if (!Physics.Raycast (transform.position, (Quaternion.Euler(0, 0, -scanAngle)*transform.forward), out hit, avoidRange*breakOutFactor)){
				breakOutAngle=scanAngle;
				return AvoidState.Right;
			}
		}
		// If no safe exit found, spin around
		return AvoidState.AboutTurn;
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

//	float AngleDir (Vector3 fwd, Vector3 targetDir, Vector3 up){
//		Vector3 perp = Vector3.Cross(fwd, targetDir);
//		float dir = Vector3.Dot(perp, up);
//
//		if (dir>0f){
//			return 1f;
//		} else if (dir <0f){
//			return -1f;
//		} else {
//			return 0f;
//		}
//	}

	float AngleDir2 (Vector2 A, Vector2 B){
		return -A.x * B.y + A.y * B.x;
	}

//	void TrackingBehaviour2(){
//		
//		Rigidbody targetRigid = target.GetComponent<Rigidbody>();
//		//Vector3 aimPoint = target.transform.position - Vector3.Normalize(targetRigid.velocity)*3;
//		Vector3 aimPoint = target.transform.position;// - targetRigid.velocity;
//		Debug.DrawLine (transform.position, aimPoint);
//
//		Vector3 desiredVel = Vector3.Normalize(aimPoint - transform.position)*1;
//		Vector3 steeringForce = desiredVel - rigidbody.velocity;
//
//
//		Debug.DrawRay (transform.position, desiredVel, Color.blue);
//		Debug.DrawRay (transform.position, steeringForce, Color.red);
//		FireThrusters();
//		if (AngleDir2 (steeringForce, transform.forward)<-0){
//			rotateInput = 1;
//			//StopThrusters();
//			Debug.Log ("Right Turn");
//		}	else if (AngleDir2 (desiredVel, transform.forward)>0){
//			rotateInput = -1;
//			//StopThrusters();
//			Debug.Log ("Left Turn");
//		} else {
//			
//		}
//		Debug.Log (AngleDir2 (steeringForce, transform.forward));
//	}

	void OnDrawGizmos(){
		foreach (Vector2 crumb in breadCrumbs){
			Gizmos.DrawSphere (crumb, 2.0f);
		}
			
			Gizmos.DrawWireSphere (lastKnownPosition, 2f);
	}

}
