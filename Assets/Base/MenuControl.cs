using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent (typeof (Image))]

public class MenuControl : MonoBehaviour {

	public float maxAlpha=0.5f;
	public float fadeSpeed=1;
	public bool fadeIn, fadeOut;
	public AudioClip[] clips;

	AudioSource audiosource;
	//Image[] images;
	//Text[] texts;
	CanvasGroup canvasGroup;
	public GameObject market, wStore, bBoard;

	enum SoundEffects {Click, Accept};

	// Use this for initialization
	void Start () {
		//images = GetComponentsInChildren<Image>();
		//texts = GetComponentsInChildren<Text>();
		audiosource=GetComponent<AudioSource>();
		canvasGroup = GetComponent<CanvasGroup>();
		//market = GameObject.FindGameObjectWithTag("Market");
		//wStore = GameObject.FindGameObjectWithTag("WStore");
		//bBoard = GameObject.FindGameObjectWithTag("BBoard");
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

		float fade = canvasGroup.alpha;
		fade = Mathf.Lerp (fade, maxAlpha, fadeSpeed*Time.deltaTime);
		if (fade >= maxAlpha*0.9){
			fadeIn=false;
		}

		canvasGroup.alpha=fade;


	}

	void FadeOut(){
		
		fadeIn=false;

		float fade = canvasGroup.alpha;
		fade = Mathf.Lerp (fade, 0, fadeSpeed*Time.deltaTime);
		if (fade <= 0.01){
			fadeOut=false;
			gameObject.SetActive(false);
		}

		canvasGroup.alpha=fade;


	}

	void Click(){
		audiosource.clip=clips[0];
		audiosource.Play();
	}

	public void ActivateBBoard(){
		Click();
		wStore.SetActive(false);
		market.SetActive(false);
		bBoard.SetActive(true);
	}

	public void ActivateWStore(){
		Click();
		wStore.SetActive(true);
		market.SetActive(false);
		bBoard.SetActive(false);
	}

	public void ActivateMarket(){
		Click();
		wStore.SetActive(false);
		market.SetActive(true);
		bBoard.SetActive(false);
	}

	public void CloseMenu(){
		Debug.Log ("Closing");
		Click();
		fadeOut=true;
		GameObject.FindObjectOfType<PlayerControl>().inMenu=false;
	}

	public void AcceptMission(){
		Debug.Log ("Accepting");
		Click();
		AudioSource musicManager=GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>();
		musicManager.Play();
		fadeOut=true;
		GameObject.FindObjectOfType<PlayerControl>().inMenu=false;
	}

}
