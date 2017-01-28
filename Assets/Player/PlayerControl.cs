using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IWeapon{
	void StartFire();
	void FireWeapon(Ray fireRay);
	void StopFire();
}

public interface IDestructable{
	void TakeFire(float damage);

}


public class PlayerControl : MonoBehaviour {

	public float thrust=1f;
	public float rotateSpeed=1f;
	public float maxSpeed=15f;
	public float maxHealth=100f;
	public Slider healthSlider;

	AudioSource audiosource;
	IWeapon weapon;
	TerrainGenerator terrain;
	Rigidbody rigidbody;
	ParticleSystem thruster;
	float health;
	bool thrusting=false;

	// Use this for initialization
	void Start () {
		rigidbody=GetComponent<Rigidbody>();
		thruster=GetComponentsInChildren<ParticleSystem>()[0];
		terrain=GameObject.FindObjectOfType<TerrainGenerator>();
		healthSlider=GameObject.FindObjectOfType<Slider>();
		weapon = GetComponentInChildren<IWeapon>();
		audiosource=GetComponent<AudioSource>();

		health=maxHealth;
		thruster.Stop();

		FindStartPosition();

	}
	
	// Update is called once per frame
	void Update () {
		// Turn left and right
		transform.Rotate (Vector3.up*Input.GetAxis("Horizontal")*rotateSpeed);

		// Fire Thrusters
		if (Input.GetButton("Fire1")){
			FireThrusters();
		} else {
			StopThrusters();
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
	}

	void StopThrusters(){
		audiosource.Stop();
		thruster.Stop();
		thrusting=false;
	}

	void FindStartPosition(){
		int startY;
		int startX=TerrainGenerator.xSizeInTiles/2;

		for (startY=1;startY<TerrainGenerator.ySizeInTiles; startY++){
			if (terrain.tileValue[startX, startY]==0){
				break;
			}
		}
		if (startY==TerrainGenerator.ySizeInTiles-1){
			Debug.Log ("No free space along entrance channel!");
		}
		else {
			transform.position=new Vector3 (startX, startY+2, 0f);
			//Debug.Log (startX +" " +startY);
			//Debug.Log ("Outside Terran, 32,0 is " +terrain.tileValue[32,0]);
		}
	}

		void OnCollisionEnter(Collision collision){
		float impactSpeed = collision.relativeVelocity.magnitude;

			if (impactSpeed>7.0){
				TakeDamage(impactSpeed, collision.collider.tag);
			}
			//Debug.Log(collision.collider.name);
			//Debug.Log(collision.collider.name);
		}

		void TakeDamage(float impactSpeed, string impactType){
			//sparks.Play();
			//Debug.Log("Smash!");
			health -= impactSpeed;

		}

		void FireWeapon(){

			


		}

}
