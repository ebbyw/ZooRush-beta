﻿using UnityEngine;
using System.Collections;

/** The Input Manager acts as the platform independent interface between the game and the input method the user is emplyoing.
 * @author: Ebtissam Wahman
 */ 
public class InputManager : MonoBehaviour
{

	private float xInput;
	private float yInput;
	private float confirm;

	private Ray origin;
	public static RaycastHit pointerTouch;

	/** The Input manager keeps track of the previous input in order to provide 
	 * a trigger based input system.
	 * 
	 * An input is only read once until the user lets go of the key or resets 
	 * the position of the joystick or removes their finger from the screen.
	 * 
	 */ 

	private  bool prevUp;
	private  bool prevDown;
	private  bool prevLeft;
	private  bool prevRight;
	private  bool prevEnter;
	
	public static bool up;
	public static bool down;
	public static bool left;
	public static bool right;
	public static bool enter;
	public static bool touching;

	void Awake ()
	{
		prevUp = false;
		prevDown = false;
		prevLeft = false;
		prevRight = false;
		prevEnter = false;
		up = false;
		down = false;
		left = false;
		right = false;
		enter = false;
		if (Input.touchCount > 0) {
			origin = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
		} else {
			origin = Camera.main.ScreenPointToRay (Input.mousePosition);
		}
		
		touching = Physics.Raycast (origin, out pointerTouch);		
	}

	void Start ()
	{
		prevUp = false;
		prevDown = false;
		prevLeft = false;
		prevRight = false;
		prevEnter = false;
		up = false;
		down = false;
		left = false;
		right = false;
		enter = false;
		if (Input.touchCount > 0) {
			origin = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
		} else {
			origin = Camera.main.ScreenPointToRay (Input.mousePosition);
		}
		
		touching = Physics.Raycast (origin, out pointerTouch);		
	}
	
	void Update ()
	{
		if (Input.touchCount > 0) {
			origin = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
		} else {
			origin = Camera.main.ScreenPointToRay (Input.mousePosition);
		}
		
		touching = Physics.Raycast (origin, out pointerTouch);
		
		xInput = Input.GetAxis ("Horizontal");
		yInput = Input.GetAxis ("Vertical");
		confirm = Input.GetAxis ("Fire1");

		changeValue (ref prevLeft, ref left, xInput < 0);
		changeValue (ref prevRight, ref right, xInput > 0);
		changeValue (ref prevUp, ref up, yInput > 0);
		changeValue (ref prevDown, ref down, yInput < 0);
		changeValue (ref prevEnter, ref enter, confirm > 0);
	}

	void changeValue (ref bool previous, ref bool current, bool pressed)
	{
		if (pressed) {//input activated
			if (!previous) { // if the input is being leaned on
				previous = true;
				current = true;
			} else { // else keep track of the input but do not echo
				previous = true;
				current = false;
			}
		} else {// otherwise keep track that the input is not active
			previous = false;
			current = false;
		}
	}

	void checkValues ()
	{
		Debug.Log (origin);
		Debug.Log ("Up: " + up);
		Debug.Log ("Down: " + down);
		Debug.Log ("Left: " + left);
		Debug.Log ("Right: " + right);
		Debug.Log ("Enter: " + enter);
	}
}