﻿using UnityEngine;
using System.Collections;

public class LevelSelectCameraControls : MonoBehaviour
{
	Camera[] cameras;
	float rightMax = 0;
	Camera rightMost;
	Camera leftMost;
	float width;

	private bool move;

	void Start ()
	{
		cameras = GameObject.FindObjectsOfType<Camera> ();
		foreach (Camera camera in cameras) {
			if (camera.rect.x > rightMax) {
				rightMax = camera.rect.x;
				rightMost = camera;
			}
			if (Mathf.Approximately (camera.rect.x, 0.025f)) {
				leftMost = camera;
			}
		}
		move = false;
		width = 0.325f;
		Debug.Log ("RIGHT MAX IS: " + rightMax);
	}
	
	void FixedUpdate ()
	{
		float xInput = Input.GetAxis ("Horizontal");
		if (xInput > 0) {
			if (!Mathf.Approximately (Mathf.Round (rightMost.rect.x * 1000), 350f)) {
				if (!move) {
					StartCoroutine (moveCameras (true));
				}
			}
		}
		if (xInput < 0) {
			if (!Mathf.Approximately (Mathf.Round (leftMost.rect.x * 1000), 350f)) {
				if (!move) {
					StartCoroutine (moveCameras (false));
				}
			}
		}
	}

	private IEnumerator moveCameras (bool left)
	{
		move = true;
		float[] newValues = new float[cameras.Length];
		float[] newValuesVelocity = new float[cameras.Length];
		float newRightMost = rightMost.rect.x - width;
		float newLeftMost = leftMost.rect.x + width;

		for (int i = 0; i < cameras.Length; i++) {
			newValues [i] = cameras [i].rect.x;
			if (left) {
				newValues [i] -= width + 0.1f;
			} else {
				newValues [i] += width + 0.1f;
			}
		}

		while (true) {
			for (int i = 0; i < cameras.Length; i++) {
				Rect temp = cameras [i].rect;
				temp.x = Mathf.SmoothDamp (temp.x, newValues [i], ref newValuesVelocity [i], 1.5f);
				cameras [i].rect = temp;
			}
			if (left) {
				if (rightMost.rect.x <= newRightMost) {
					for (int i = 0; i < cameras.Length; i++) {
						Rect temp = cameras [i].rect;
						temp.x = newValues [i] + 0.1f;
						cameras [i].rect = temp;
					}
					break;
				}
			} else {
				if (leftMost.rect.x >= newLeftMost) {
					for (int i = 0; i < cameras.Length; i++) {
						Rect temp = cameras [i].rect;
						temp.x = newValues [i] - 0.1f;
						cameras [i].rect = temp;
					}
					break;
				}
			}
		}

		yield return new WaitForSeconds (0.3f);
		move = false;
	}
}