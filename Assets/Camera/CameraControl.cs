using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public float levelBase=15.0f;
	public float baseZoom = 30.0f;
	public float scaleFactor = 4.0f;
	public float zoomSpeed = 0.2f;

	private Camera camera;
	private GameObject player;
	private Rigidbody playerBody;

	// Use this for initialization

	void Start () {
		camera=GetComponent<Camera>();
		player=GameObject.FindGameObjectWithTag("Player");
		playerBody=player.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		float camerax=player.transform.position.x;
		float cameray=Mathf.Clamp(player.transform.position.y, levelBase, 1000);
		float cameraz=Mathf.Lerp (-transform.position.z, baseZoom+playerBody.velocity.magnitude/scaleFactor,zoomSpeed*Time.deltaTime);
		transform.position = new Vector3 (camerax, cameray, -cameraz);
	}

	float ZoomOut(float speed){
		return 0f;
	}
}
