﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : MonoBehaviour, IWeapon {

	public float range=20f;
	public float damage=0.1f;
	public GameObject sparks;
	public AudioClip[] clips;

	AudioSource audiosource;
	float startWindupTime;
	bool firing=false;

	// Use this for initialization
	void Start () {
		audiosource=GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public float Range(){
		return range;
	}

	public void StartFire(){
		audiosource.clip=clips[0];
		audiosource.Play();
		startWindupTime = Time.time;
	}

	public void FireWeapon(Ray gunRay){
			
			// Only start firing if gun has had enough time to windup
			if (Time.time-startWindupTime>=clips[0].length && startWindupTime!=0){

				RaycastHit hitInfo;
				//Vector3 dest = transform.position+ transform.forward*20.0f;
				//Debug.Log ("Position: " + transform.position + "    Forward Vector: " + dest);
				Debug.DrawRay (transform.position, gunRay.direction*20, Color.white);
	
				if (Physics.Raycast (gunRay, out hitInfo, range)){
					//Debug.Log (hitInfo.transform.name);
					//Debug.Log (hitInfo.collider.name);
					GameObject tempSparks = Instantiate (sparks, hitInfo.point, Quaternion.identity);
					Destroy (tempSparks, 0.3f);
					if (hitInfo.collider.GetComponent<IDestructable>()!=null){
						IDestructable target = hitInfo.collider.GetComponent<IDestructable>();
						target.TakeFire(damage*Time.deltaTime, hitInfo);
					}

				}

				//If not already doing so, start making noise
				if (!firing){
					audiosource.clip=clips[1];
					audiosource.loop=true;
					audiosource.Play();
					firing=true;
				}
			}
	}

	public void StopFire(){
		audiosource.clip=clips[2];
		audiosource.loop=false;
		audiosource.Play();
		startWindupTime = 0;
		firing=false;
	}

}
