﻿using UnityEngine;
using System.Collections;

public class EndlessModePainKiller : MonoBehaviour, OtherButtonClass
{

		private int pillCount; // current count of Pills
		private EndlessPainIndicator painIndicator; //pointer to the Pain Indicator 
		private EndlessSceneManager sceneManager; //pointer to scene manager
		private ScoreKeeper scoreKeeper; //pointer to score keeper
		private int painPoints = 100; //amount of points we subtract from the pain indicator
		private bool interacted; //if an interaction has been registered
		public AudioClip clip; //audio clip that plays on clicking the pill
		private AudioController audioController; //pointer to the scene audio controller
		private Animator animator; //Animator for +/- 1 effect when interecting with painkiller button & doctors
		public TextMesh animationText; //Text that is animated after interaction
		private CharacterSpeech characterSpeech; //pointer to character speech bubble handler
	
		void Start ()
		{
				scoreKeeper = GameObject.FindObjectOfType<ScoreKeeper> ();
				sceneManager = GameObject.FindObjectOfType<EndlessSceneManager> ();
				painIndicator = GameObject.FindObjectOfType<EndlessPainIndicator> ();
				audioController = GameObject.FindObjectOfType<AudioController> ();
				characterSpeech = GameObject.FindObjectOfType<CharacterSpeech> ();
				animator = GetComponent<Animator> ();
				pillCount = 0;//at the start of the level we get our current pill count
				sceneManager.updatePillCount (pillCount);
		}
	
		/**
	 * When the pain killer button is pressed, we want to update the pill count,
	 * update the pain indicator, and add the pill usage to the score keeper.
	 */ 
		public void otherButtonAction (Button thisButton)
		{
				if (!interacted && Input.GetMouseButtonUp (0)) {
						interacted = true;
						decrementPillCount ();
						StartCoroutine (waitToResetTouch ()); //resets the intereacted tracker after a small period of time
				}
		}
	
		public void incrementPillCount ()
		{
				characterSpeech.SpeechBubbleDisplay ("Thanks Doc!");
				if (pillCount < 3) {
						++pillCount;
						animationText.text = "+1";
						animator.SetTrigger ("Increment");
				}
		}
	
		private void increment ()
		{
				sceneManager.updatePillCount (pillCount);
				animator.SetTrigger ("Reset");
		}
	
		private void decrementPillCount ()
		{
				if (pillCount > 0) {
						--pillCount;
						characterSpeech.SpeechBubbleDisplay ("What a Relief!");
						animationText.text = "-1";
						animator.SetTrigger ("Decrement");
				}
		}
	
		private void decrement ()
		{
				audioController.objectInteraction (clip);
		
				sceneManager.updatePillCount (pillCount); //tells the scene manager to update our pill count
				if (scoreKeeper) {
						scoreKeeper.addToCount ("Pill");
				}
				painIndicator.subtractPoints (painPoints);
				animator.SetTrigger ("Reset");
		
		}

	
		private IEnumerator waitToResetTouch ()
		{
				yield return new WaitForSeconds (0.15f);
				interacted = false;
		}
}
