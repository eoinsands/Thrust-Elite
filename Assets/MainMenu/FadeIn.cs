using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour {

	Text text;
	float alpha;
	Color color;
	public float fadeSpeed = 0.5f;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		color = text.color;
		color.a = 0;
		text.color = color;
	}
	
	// Update is called once per frame
	void Update () {
		alpha = Mathf.Lerp(alpha, 1.0f, fadeSpeed*Time.deltaTime);
		color.a = alpha;
		text.color=color;
	}
}
