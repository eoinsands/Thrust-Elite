using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour, IDestructable {

	public float maxShield=10f;
	public float shieldRegenDelay=2f;
	public float shieldRestartDelay=5f;
	public float shieldRegenRate=.5f;
	public Color shieldColor = Color.white;
    public GameObject collisionLight;
    public float shield;
    public float lastHit;
    public Slider shieldSlider;
	
	SphereCollider collider;
	PlayerControl player;

	// Use this for initialization
	void Start () {
		shield=maxShield;

		//shieldSlider=GameObject.FindObjectsOfType<Slider>()[1];
		shieldSlider.maxValue=maxShield;
		//player=GetComponentInParent<PlayerControl>();
		collider=GetComponent<SphereCollider>();

	}
	
	// Update is called once per frame
	void Update () {
	//If shields aren't full
		if (shield<maxShield){
			//If shields are down
			if (shield<=0){
				if ((Time.time-lastHit)>=shieldRestartDelay){
					shield+=shieldRegenRate*Time.deltaTime;
					//player.shieldsUp=true;
					collider.enabled=true;
					GetComponent<MeshRenderer>().enabled=true;
				}
			}
			else {
				if ((Time.time-lastHit)>=shieldRegenDelay){
					shield+=shieldRegenRate*Time.deltaTime;
				}
			}
		}

		shieldSlider.value=shield;
	}

	public void TakeFire(float damage, RaycastHit hitInfo){
		Debug.Log ("Shield hit!");
		lastHit=Time.time;
		shield-=damage;
		OnHit (hitInfo);
		if (shield<=0){
			//player.shieldsUp=false;
			shield=0;
			collider.enabled=false;
			GetComponent<MeshRenderer>().enabled=false;
		}
	}

	//For manual activation through script (good for weapon hits). Same as above, but only spawns one light.
    public void OnHit (RaycastHit hit)
    {
        Light light = (Instantiate (collisionLight, hit.point + (hit.normal * 0.5f), Quaternion.identity)as GameObject).GetComponent<Light> ();
        light.color = shieldColor;
        light.type = LightType.Spot;
        light.bounceIntensity = 0;
        light.intensity = 10;
        light.range = 5;
        light.spotAngle = 75;
        light.transform.SetParent (transform);
        light.transform.LookAt (hit.point);
 
    }
}
