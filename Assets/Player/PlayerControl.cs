using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public interface IWeapon{
	void StartFire();
	void FireWeapon(Ray fireRay);
	void StopFire();
	float Range();
}

public interface IDestructable{
	void TakeFire(float damage, RaycastHit hitInfo);

}


public class PlayerControl : MonoBehaviour, IDestructable {

	public float thrust=1f;
	public float rotateSpeed=1f;
	public float maxSpeed=15f;
	public float maxHealth=100f;
	public Slider healthSlider;
	public bool inMenu=false;
	public bool shieldsUp;

	AudioSource audiosource;
	IWeapon weapon;
	TerrainGenerator terrain;
	Rigidbody rigidbody;
	ParticleSystem thruster;
	Shield shield;
	public float health;
	bool thrusting=false;

	// Use this for initialization
	void Start () {
		Object.DontDestroyOnLoad(gameObject);
		rigidbody=GetComponent<Rigidbody>();
		thruster=GetComponentsInChildren<ParticleSystem>()[0];
		shield=GetComponentInChildren<Shield>();
		terrain=GameObject.FindObjectOfType<TerrainGenerator>();
		//healthSlider=GameObject.FindObjectsOfType<Slider>()[0];
		weapon = GetComponentInChildren<IWeapon>();
		audiosource=GetComponent<AudioSource>();

		health=maxHealth;
		healthSlider.maxValue=maxHealth;

		thruster.Stop();

		FindStartPosition();
		rigidbody.useGravity=true;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.DrawRay (transform.position, (Quaternion.Euler(0f, 0f, 15f)*transform.forward), Color.red);

		if (!inMenu){
			// Turn left and right
			transform.Rotate (Vector3.up*Input.GetAxis("Horizontal")*rotateSpeed);

			// Fire Thrusters
			if (Input.GetButton("Fire1")){
				FireThrusters();
			} else {
				StopThrusters();
			}

			// Hover Mode
			if (Input.GetButtonDown("Fire3")){
				rigidbody.useGravity = !rigidbody.useGravity;
				Debug.Log ("Gravity off!");
			}

			// Fire Guns
			if (Input.GetButtonDown("Fire2")){
				weapon.StartFire();
			} else if (Input.GetButton("Fire2")){
				weapon.FireWeapon(new Ray(transform.position, transform.forward));
			}

			if (Input.GetButtonUp("Fire2")){
				weapon.StopFire();
			}
		}


		//Clamp Max Speed
		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);

		healthSlider.value=health;
	}

	void FireThrusters(){
		//Debug.Log ("Firing");
		thruster.Play();
		if (!thrusting){
			audiosource.Play();
			thrusting=true;
		}
		//Debug.Log(transform.forward);
		rigidbody.AddForce(transform.forward*thrust);
		//Once you start thrusting, antigrav is disabled
		rigidbody.useGravity = true;
	}

	void StopThrusters(){
		audiosource.Stop();
		thruster.Stop();
		thrusting=false;
	}

	void FindStartPosition(){
		// Find landing platform and place ship above it
		GameObject landPlatform = GameObject.FindGameObjectWithTag("Platform");
		transform.position = landPlatform.transform.position+ new Vector3 (0, 0.9f, -0.65f);

	}

	void OnTriggerEnter (Collider collider){
		if (collider.transform.tag=="Gate"){
			GateJump();
		}
	}

	void OnCollisionEnter(Collision collision){
		float impactSpeed = collision.relativeVelocity.magnitude;
			
		if (impactSpeed>7.0){
			//TakeCollisionDamage(impactSpeed, collision.collider.tag);
		}
			//Debug.Log(collision.collider.name);
			//Debug.Log(collision.collider.name);
	}

	void TakeCollisionDamage(float impactSpeed, string impactType){
			//sparks.Play();
			//Debug.Log("Smash!");
			health -= impactSpeed;

	}

	public void TakeFire(float damage, RaycastHit hitInfo){
		
		health-=damage;
		shield.lastHit = Time.time;
		if (health<=0){
			Destroy(gameObject);
		}
		
	}

	void GateJump(){
		Debug.Log ("Jump Activated");
		int chosenGate=2;
		if ((chosenGate!=0)&&(chosenGate!=SceneManager.GetActiveScene().buildIndex)){
			SceneManager.LoadScene(chosenGate);
		}
	}

}
