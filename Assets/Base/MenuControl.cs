using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent (typeof (Image))]

public class MenuControl : MonoBehaviour {

	public float maxAlpha=0.5f;
	public float fadeSpeed=1;

	Image[] images;
	Text[] texts;
	public bool fadeIn, fadeOut;

	// Use this for initialization
	void Start () {
		images = GetComponentsInChildren<Image>();
		texts = GetComponentsInChildren<Text>();
		//gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (fadeIn){
			FadeIn();
		}

		else if (fadeOut){
			FadeOut();
		}
	}

	void FadeIn(){
		if (!gameObject.activeSelf){
			gameObject.SetActive (true);
		}
		fadeOut=false;
		Color color = images[0].color;
		color.a = Mathf.Lerp (color.a, maxAlpha, fadeSpeed*Time.deltaTime);
		if (color.a >= maxAlpha*0.9){
			fadeIn=false;
		}

		foreach (Image image in images){
			image.color=color;
		}
		foreach (Text text in texts){
			text.color=color;
		}

	}

	void FadeOut(){
		fadeIn=false;
		Color color = images[0].color;
		color.a = Mathf.Lerp (color.a, 0, fadeSpeed*Time.deltaTime);

		if (color.a <= 0.01){
			fadeOut=false;
			gameObject.SetActive (false);
		}
		foreach (Image image in images){
			image.color=color;
		}
		foreach (Text text in texts){
			text.color=color;
		}

	}

	public void CloseMenu(){
		fadeOut=true;
		GameObject.FindObjectOfType<PlayerControl>().inMenu=false;
	}

}
