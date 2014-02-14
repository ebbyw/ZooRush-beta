﻿using UnityEngine;
using System.Collections;

/** Data and functions specific to Animal objects.
 * 
 * @author Ebtissam Wahman
 */ 
public class Animal : MonoBehaviour
{

	public bool caught; //Indicator for whehter the Animal has been caught by the player
	public Vector2 speed; //Current speed of the animal object
	public Sprite animalIcon; //Icon used in the distance meter
	private Animator animator; //Animator for the animal's running sprites
	private SceneManager sceneManager; // Pointer to the scene manager
	private bool play; //Current frame's state of whether the game is in play
	private bool prevPlay; //Previous frame's state of whether the game is in play

	void Start ()
	{
		sceneManager = GameObject.FindObjectOfType<SceneManager> (); //assigns the pointer to the scene manager
		animator = GetComponent<Animator> (); //assigns the pointer to the animator component
		caught = false; //default value for whether the animal has been caught
		transform.parent.rigidbody2D.velocity = new Vector2 (0f, 0f);
		/** The "play" and "prevPlay" boolean values are used to create an 
		* "in play" or "not in play" state machine so that we may be able to 
		* properly manage the movement and functionality of the animal object.
		*/
		play = sceneManager.isPlaying; //the current value the game's play state
		prevPlay = play; //at the beginninge the previous and current are equal
		GameObject.Find ("Animal Icon").GetComponent<SpriteRenderer> ().sprite = animalIcon; //Changes the icon in the distance meter
	}
	
	void Update ()
	{
		animator.SetFloat ("Speed", transform.parent.rigidbody2D.velocity.x); //Tells the animator state machine what the current speed value is
		prevPlay = play; //updates the previous frame's state
		play = sceneManager.isPlaying; //updates the current frame's state
		if (sceneManager.startPressed) {
			if (!prevPlay && play) { //Our previous state is the paused state, we are now going into the play state
				StartCoroutine (waitToResume (0.1f));
			} else { // our previous state was the play state
				if (!play) {//we need to move into the paused state
					transform.parent.rigidbody2D.velocity = new Vector2 (0f, 0f);
				}
			}
		}
	}

	/**
	 * Sets the speed to the animal's standard running speed.
	 */ 
	public void setSpeed ()
	{
		transform.parent.rigidbody2D.velocity = speed; //assigns the rigidbody component the desired velocity
	}

	/** 
	 * Randomly changes the y-axis value of the animal object.
	 */
	public void changeY ()
	{
		int moveChance = Random.Range (1, 101);
		if (moveChance % 100 == 0) {//1% chance
			float yForce = Random.Range (-150f, 150f);
			transform.parent.rigidbody2D.AddForce (new Vector2 (0, yForce));
		}

	}
	
	/** Resumes the default speed of the animal.
	* @param time the wait time before resetting the speed of the animal
	*/
	
	private IEnumerator waitToResume (float time)
	{
		yield return new WaitForSeconds (time);
		transform.parent.rigidbody2D.velocity = speed;
	}
}
