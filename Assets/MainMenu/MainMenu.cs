using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	bool fadeOut;
	public GameObject panel;
	Image image;
	Color color;
	float alpha;
	public float fadeSpeed = 0.5f;

	// Use this for initialization
	void Start () {
		image = panel.GetComponent<Image>();
		color = image.color;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (fadeOut){
			FadeOut();
		}
	}

	void FadeOut(){
		Debug.Log ("Fading");
		alpha = Mathf.Lerp(image.color.a, 1.0f, fadeSpeed*Time.deltaTime);
		color.a=alpha;
		image.color = color;
	}

	public void LoadLevel (int level){
		Debug.Log ("Loading Level " + level);
		fadeOut=true;
		SceneManager.LoadScene(level);

	}

}
