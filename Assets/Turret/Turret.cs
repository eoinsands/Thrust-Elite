using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour, IDestructable {

	public float health=10.0f;
	public AudioClip[] clips;

	float audioDelay=0;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource=GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void playRicochet(){
		if (audioDelay<=0){
			int ricNoise=Random.Range(0, 50);
			if (ricNoise<=clips.Length){
				audioSource.clip=clips[ricNoise];
				audioSource.Play();
				audioDelay=clips[ricNoise].length;
			}
		} else {
			audioDelay-=Time.deltaTime;
		}

	}

	public void TakeFire(float damage, RaycastHit hitInfo){
		
		health-=damage;
		if (health<=0){
			Destroy(gameObject);
		}
	}
}
