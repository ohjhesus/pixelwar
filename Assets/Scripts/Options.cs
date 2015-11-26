using UnityEngine;
using System.Collections;

public class Options : MonoBehaviour {

	public float sfxVolume;
	public float musicVolume;
	public Vector2 resolution;
	public bool isFullscreen;
	private string[] tempResolution;
	private string originalResolution;

	// Use this for initialization
	void Start () {
		sfxVolume = PlayerPrefs.GetFloat ("sfxVolume", 0.5f);
		musicVolume = PlayerPrefs.GetFloat ("musicVolume", 0.5f);
		GetSS ();

		UpdateSettings ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GetSS () {
		originalResolution = Screen.currentResolution.width.ToString () + "." + Screen.currentResolution.height.ToString ();
		tempResolution = PlayerPrefs.GetString ("resolution", originalResolution).Split("."[0]);
		resolution = new Vector2 (int.Parse(tempResolution [0]), int.Parse(tempResolution [1]));
		Debug.Log ("Resoltion: " + resolution);

		if (PlayerPrefs.HasKey ("fullscreen") && PlayerPrefs.GetInt ("fullscreen") == 0) {
			isFullscreen = false;
		} else {
			isFullscreen = true;
		}
	}

	void UpdateSettings () {
		if (Application.platform == RuntimePlatform.OSXEditor)
			return;
		Screen.SetResolution (Mathf.FloorToInt (resolution.x), Mathf.FloorToInt (resolution.y), isFullscreen);
	}
}