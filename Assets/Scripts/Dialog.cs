﻿using UnityEngine;
using System.Collections;

public class Dialog : Button
{

	public bool tutorialOnly;

	public string[] text;

	private bool opened, over, click;
	private int currentTextIndex;
	private Animator animator;

	void Start ()
	{
		opened = over = click = false;
		animator = GetComponent<Animator> ();
	}
	
	void Update ()
	{
	
	}

	public void stopSpeaking ()
	{
		animator.SetBool ("Speak", false);
	}


	public void open ()
	{
		Debug.Log ("OPEN CALLED");
		animator.SetTrigger ("Opened");
		animator.SetBool ("Speak", true);
		string displayText;
		if (text [0] [0] == '+') {
			displayText = text [0].Substring (1);
		} else {
			displayText = text [0];
		}
		if (text.Length > 1 && text [1] [0] == '+') {
			displayText += "\n" + text [1].Substring (1);
			currentTextIndex = 2;
			if (text.Length > 2 && text [2] [0] == '+') {
				displayText += "\n" + text [2].Substring (1);
				currentTextIndex = 3;
			}
		}
		GetComponentInChildren<TextAnimator> ().AnimateText (displayText);
	}

	private void next ()
	{
		animator.SetBool ("Speak", true);
		string displayText;
		if (text [currentTextIndex] [0] == '+') {
			displayText = text [currentTextIndex].Substring (1);
		} else {
			displayText = text [currentTextIndex];
		}
		if (text.Length > currentTextIndex + 1 && text [currentTextIndex + 1] [0] == '+') {
			displayText += "\n" + text [currentTextIndex + 1].Substring (1);
			currentTextIndex = currentTextIndex + 2;
			if (text.Length > currentTextIndex + 2 && text [currentTextIndex + 2] [0] == '+') {
				displayText += "\n" + text [currentTextIndex + 2].Substring (1);
				currentTextIndex = currentTextIndex + 3;
			}
		}
		GetComponentInChildren<TextAnimator> ().AnimateText (displayText);

	}

	private void close ()
	{

	}

	protected override void action ()
	{
		Debug.Log ("CLICKED");
		if (currentTextIndex < text.Length) {
			next ();
		} else {
			close ();
		}
	}

}