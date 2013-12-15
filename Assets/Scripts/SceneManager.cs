using UnityEngine;
using System.Collections;


/** Main controller for most level-based events.
 * 
 * @author: Ebtissam Wahman
 */ 
public class SceneManager : MonoBehaviour
{
	public string NextSceneName;
	public float distanceDiffMin;
	public float currentDistanceDiff;
	public float waitTime;
	public int levelNumber;
	public float targetTimeVar;
	public float multiplier1;
	public float multiplier2;
	public GameObject[] menus;
	private GameObject screenDimmer;
	
	private PlayerControls playerControl;
	private ScoreKeeper scoreKeeper;
	private Animal animalControl;

	private GameObject character;
	private GameObject animal;
	private GameObject painBar;
	private GameObject timeIndicator;
	private NetLauncher netLauncher;
	
	public bool levelStartWait;
	public bool isPlaying;
	public bool tutEnabled;
	public bool fainted;
	public bool hitByVehicle;
	
	private bool starDisplay1;
	private bool starDisplay2;
	
	private bool scoreDisplayed;
	private bool timeCountDown;
	private int time;
	private int infections;
	private int powerUps;
	private int starCount;
	private int stars;
			
	void Start ()
	{
		isPlaying = true;
		levelStartWait = true;
		fainted = false;
		hitByVehicle = false;
		scoreDisplayed = false;
		timeCountDown = true;
		starDisplay1 = false;
		starDisplay2 = false;
		stars = 3;
		playerControl = GameObject.FindObjectOfType<PlayerControls> ();
		scoreKeeper = GameObject.FindObjectOfType<ScoreKeeper> ();
		animalControl = GameObject.FindObjectOfType<Animal> ();
		netLauncher = GameObject.FindObjectOfType<NetLauncher> ();
		character = GameObject.FindGameObjectWithTag ("character");
		animal = GameObject.FindGameObjectWithTag ("animal");
		
		screenDimmer = GameObject.Find ("GUI - Level Dimmer");
		
		distanceDiffMin = 6.5f;
		currentDistanceDiff = Mathf.Abs (animal.transform.position.x - character.transform.position.x);
		painBar = GameObject.Find ("Pain Bar");
		timeIndicator = GameObject.Find ("GUI - Time");
//		if (!PlayerPrefs.HasKey ("Tutorial")) {
//			PlayerPrefs.SetString ("Tutorial", "true");
//		}
//		tutEnabled = ((PlayerPrefs.GetString ("Tutorial").Equals ("true")) ? true : false);
	}
	
	void Update ()
	{
		currentDistanceDiff = Mathf.Abs (animal.transform.localPosition.x - character.transform.localPosition.x);
		if (levelStartWait) {
			if (currentDistanceDiff > 18f) {
				levelStartWait = false;
			}
			
		} else {
			if (isPlaying) {
				if (animalControl.caught) {
					isPlaying = false;
					if (!scoreDisplayed) {
						StartCoroutine (displayScore ());
					}
				} else {
					if (currentDistanceDiff < distanceDiffMin) {
						playerControl.setSpeed (animalControl.speed);
						netLauncher.launchEnabled = true;
					} else {
						netLauncher.launchEnabled = false;
					}
					if (fainted) {
						isPlaying = false;
						StartCoroutine (displayFainted ());
					} else {
						if (hitByVehicle) {
							isPlaying = false;
							StartCoroutine (displayGotHit ());
						} else {
							if (currentDistanceDiff < distanceDiffMin) {
								playerControl.setSpeed (animalControl.speed);
								netLauncher.launchEnabled = true;
							} else {
								netLauncher.launchEnabled = false;
							}
						}
					}
					
				}
			}
		}
		
		
	}
	
	void FixedUpdate ()
	{
		if (timeCountDown) {
			if (time > 0) {
				StartCoroutine (starDisplay (stars));
				StartCoroutine (timeCountDownMethod ());
			} else {
				timeCountDown = false;
				if (!starDisplay1) {
					if (time - targetTimeVar > 0) {
						stars--;
					}
					if (time - (multiplier1 * targetTimeVar) > 0) {
						stars--;
					}
					if (time - (multiplier2 * targetTimeVar) > 0) {
						stars--;
					}
					StartCoroutine (starDisplay (stars));
				} else {
					if (infections > 0) {
						StartCoroutine (infectionCountDown ());
					} else {
						if (!starDisplay2) {
							if (stars > 0) {
								if (infections > powerUps) {
									stars--;
								}
							}
							StartCoroutine (starDisplay (stars));
						}
					}
				}
			}
			
		}
	}
	
	private IEnumerator infectionCountDown ()
	{
		infections -= 1;
		yield return new WaitForSeconds (0.3f);
		GameObject.Find ("Menu - Infect Count").GetComponent<TextMesh> ().text = "" + infections;
	}
	
	private IEnumerator starDisplay (int starNumber)
	{
		if (time <= 0) {
			if (starDisplay1 == false) {
				starDisplay1 = true;
			} else {
				starDisplay2 = true;
			}
		}
		yield return new WaitForSeconds (1f);
		
		if (starNumber > 0) {//# of stars > 0
			Animator[] stars = GetComponentsInChildren<Animator> ();
			foreach (Animator star in stars) {
				if (star.name.Contains ("Star 1")) {
					if (starCount >= 1) {
						star.SetTrigger ("Activate");
					} else {
						star.SetTrigger ("Deactivate");
					}
				}
				if (star.name.Contains ("Star 2")) {
					if (starCount >= 2) {
						star.SetTrigger ("Activate");
					} else {
						star.SetTrigger ("Deactivate");
					}
				}
				if (star.name.Contains ("Star 3")) {
					if (starCount >= 3) {
						star.SetTrigger ("Activate");
					} else {
						star.SetTrigger ("Deactivate");
					}
				}
			}
		}
	}
	
	private IEnumerator timeCountDownMethod ()
	{
		time -= 1;
		yield return new WaitForSeconds (0.1f);
		TextMesh timeTextMesh = GameObject.Find ("Menu - Time").GetComponent<TextMesh> ();
		int minutes = time / 60;
		int seconds = time % 60;
		string timeText = "";
		if (minutes % 100 <= 9 && minutes <= 99) {
			timeText = "0" + minutes;
		} else {
			if (minutes <= 99) {
				timeText = "" + minutes;
			} else {
				timeText = "99";
			}
		}
		
		timeText += ":";
		
		if (seconds % 100 <= 9 && minutes <= 100f) {
			timeText += "0" + seconds;
		} else {
			if (minutes <= 100f) {
				timeText += "" + seconds;
			} else {
				timeText += "59+";
			}
		}
		timeTextMesh.text = timeText;
	}
	
	private IEnumerator displayGotHit ()
	{
		yield return new WaitForSeconds (0.1f);
		dimScreen ();
		if (GameObject.Find ("GUI Menu - Fainted(Clone)") == null) {
			GameObject menu = GameObject.Find (menus [1].name);
			if (menu == null) {
				menu = Instantiate (menus [1]) as GameObject;
				GameObject.Find ("Menu - Title Text").GetComponent<TextMesh> ().text = "You Got Hit!";
				menu.transform.parent = Camera.main.transform;
				menu.transform.localPosition = new Vector3 (0f, 0f, 10f);
			}
		}
	}
	
	private IEnumerator displayFainted ()
	{
		yield return new WaitForSeconds (0.1f);
		dimScreen ();
		if (GameObject.Find ("GUI Menu - Fainted(Clone)") == null) {
			GameObject menu = GameObject.Find (menus [1].name);
			if (menu == null) {
				menu = Instantiate (menus [1]) as GameObject;
				menu.transform.parent = Camera.main.transform;
				menu.transform.localPosition = new Vector3 (0f, 0f, 10f);
			}
		}
	}

	private IEnumerator displayScore ()
	{
		scoreDisplayed = true;
		yield return new WaitForSeconds (1f);
		dimScreen ();
		
		int[] scores = scoreKeeper.getScore ();
		scoreStore (scores);
		infections = scores [0] + scores [1] + scores [2];
		powerUps = scores [3] + scores [4];
		GameObject menu = GameObject.Find (menus [2].name);
		if (menu == null) {
			menu = Instantiate (menus [2]) as GameObject;
			menu.transform.parent = Camera.main.transform;
			menu.transform.localPosition = new Vector3 (0f, 0f, 10f);
		}
		
		TextMesh[] menuText = menu.GetComponentsInChildren<TextMesh> ();
		foreach (TextMesh text in menuText) {
			if (text.name.Contains ("Next Level")) {
				text.gameObject.GetComponent<TextOption> ().levelName = NextSceneName;
			}
			
			if (text.name.Contains ("Infect Count")) {
				text.text = "" + infections;
			}
			
			if (text.name.Contains ("Time")) {
				time = scores [5];
				int minutes = scores [5] / 60;
				int seconds = scores [5] % 60;
				string timeText = "";
				if (minutes % 100 <= 9 && minutes <= 99) {
					timeText = "0" + minutes;
				} else {
					if (minutes <= 99) {
						timeText = "" + minutes;
					} else {
						timeText = "99";
					}
				}
				
				timeText += ":";
				
				if (seconds % 100 <= 9 && minutes <= 100f) {
					timeText += "0" + seconds;
				} else {
					if (minutes <= 100f) {
						timeText += "" + seconds;
					} else {
						timeText += "59+";
					}
				}
				text.text = timeText;
			}
		}
		
	
		Animator[] stars = GetComponentsInChildren<Animator> ();
		foreach (Animator star in stars) {
			if (star.name.Contains ("Star 1")) {
				star.SetTrigger ("Activate");
			}
			if (star.name.Contains ("Star 2")) {
				star.SetTrigger ("Activate");
			}
			if (star.name.Contains ("Star 3")) {
				star.SetTrigger ("Activate");
			}
		}
		
		starCount = scores [6];
		unlockLevel ();
		timeCountDown = true;
	}
	
	private void scoreStore (int[] score)
	{
		if (PlayerPrefs.HasKey (Application.loadedLevelName + "Stars")) {
			int currentHigh = PlayerPrefs.GetInt (Application.loadedLevelName + "Stars");
			if (currentHigh < score [6]) {
				PlayerPrefs.SetInt (Application.loadedLevelName + "Stars", score [6]);
				PlayerPrefs.SetInt (Application.loadedLevelName + "RedInfections", score [0]);
				PlayerPrefs.SetInt (Application.loadedLevelName + "YellowInfections", score [1]);
				PlayerPrefs.SetInt (Application.loadedLevelName + "GreenInfections", score [2]);
				PlayerPrefs.SetInt (Application.loadedLevelName + "WaterBottles", score [3]);
				PlayerPrefs.SetInt (Application.loadedLevelName + "PillBottle", score [4]);
				PlayerPrefs.SetInt (Application.loadedLevelName + "Time", score [5]);
			}
		} else {
			PlayerPrefs.SetInt (Application.loadedLevelName + "Stars", score [6]);
			PlayerPrefs.SetInt (Application.loadedLevelName + "RedInfections", score [0]);
			PlayerPrefs.SetInt (Application.loadedLevelName + "YellowInfections", score [1]);
			PlayerPrefs.SetInt (Application.loadedLevelName + "GreenInfections", score [2]);
			PlayerPrefs.SetInt (Application.loadedLevelName + "WaterBottles", score [3]);
			PlayerPrefs.SetInt (Application.loadedLevelName + "PillBottle", score [4]);
			PlayerPrefs.SetInt (Application.loadedLevelName + "Time", score [5]);
		}
	}

	private void dimScreen ()
	{
//		timeIndicator.SetActive (false);
		painBar.SetActive (false);
		screenDimmer.GetComponent<SpriteRenderer> ().enabled = true;
	}
	
	private void lightScreen ()
	{
		timeIndicator.SetActive (true);
		painBar.SetActive (true);
		screenDimmer.GetComponent<SpriteRenderer> ().enabled = false;
	}

	private void unlockLevel ()
	{
		PlayerPrefs.SetString (NextSceneName, "true");
	}

}


