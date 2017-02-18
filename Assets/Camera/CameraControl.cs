using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public float levelBase=-113.0f;
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

		if (camera.orthographic){
			float scale=OrthoScale();
			transform.position = new Vector3 (camerax, cameray, -64);
			camera.orthographicSize=scale;
		}
		else {
			float cameraz=PerspScale();
			transform.position = new Vector3 (camerax, cameray, -cameraz);
		}

	}

	float PerspScale(){
		return Mathf.Lerp (-transform.position.z, baseZoom+playerBody.velocity.magnitude/scaleFactor,zoomSpeed*Time.deltaTime);
	}

	float OrthoScale(){
		return Mathf.Lerp (camera.orthographicSize, baseZoom+playerBody.velocity.magnitude/(scaleFactor*2),zoomSpeed*Time.deltaTime);
	}

	float ZoomOut(float speed){
		return 0f;
	}
}
