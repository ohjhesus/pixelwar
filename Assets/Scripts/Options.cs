using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class Options : MonoBehaviour {

	/* LIST OF PLAYERPREFS:
	 *
	 * sfxVolume
	 * musicVolume
	 *
	 * resolution
	 * vsync
	 * aa - antialiasing
	 * ca - chromatic aberration
	 * motionBlur
	 * bloom
	 *
	 * displayNameTags
	 * displayNoobHints
	 */

	[HideInInspector]public float sfxVolume;
	[HideInInspector]public float musicVolume;

	[HideInInspector]public Vector2 resolution;
	private string[] tempResolution;
	private string originalResolution;
	[HideInInspector]public bool isFullscreen;
	[HideInInspector]public bool vsync;
	[HideInInspector]public int aa;

	[HideInInspector]public bool displayNameTags;
	[HideInInspector]public bool displayNoobHints;
	public GameObject fpsDisplay;
    [HideInInspector]public bool displayFPS;

	[HideInInspector]public bool motionBlur;
	[HideInInspector]public bool bloom;
	[HideInInspector]public bool chromaticAberration;

	public AudioMixer mixer;

    void Start () {
        UpdateSettings();
    }

	void Update () {
		if ((Input.GetKey (KeyCode.LeftCommand) || Input.GetKey (KeyCode.LeftControl)) && Input.GetKeyDown (KeyCode.R)) {
			PlayerPrefs.SetString ("resolution", "2560x1600");
			UpdateSettings ();
		}
	}

	public static bool GetBool (string var, bool defaultValue) {
		if (PlayerPrefs.HasKey (var)) {
			if (PlayerPrefs.GetInt (var) == 0) {
				return false;
			} else {
				return true;
			}
		} else {
			return defaultValue;
		}
	}

	public bool GetBool (string var) {
		if (PlayerPrefs.GetInt (var) == 0) {
			return false;
		} else {
			return true;
		}
	}

	public static void SetBool (string var, bool value) {
		if (!value) {
			PlayerPrefs.SetInt (var, 0);
		} else {
			PlayerPrefs.SetInt (var, 1);
		}
	}

	public void SetPPInt (string v, int i) {
		Debug.Log ("Setting PlayerPrefs int: " + v + ", " + i);
		PlayerPrefs.SetInt (v, i);
	}

	public void SetPPFloat (string v, float f) {
		Debug.Log ("Setting PlayerPrefs float: " + v + ", " + f);
		PlayerPrefs.SetFloat (v, f);
	}

	public void SetPPString (string v, string s) {
		Debug.Log ("Setting PlayerPrefs string: " + v + ", " + s);
		PlayerPrefs.SetString (v, s);
	}

	public void SetPPBool (string v, bool b) {
		Debug.Log ("Setting PlayerPrefs bool: " + v + ", " + b);
		SetBool (v, b);
	}

	void GetSS () {
		originalResolution = Screen.currentResolution.width.ToString () + "x" + Screen.currentResolution.height.ToString ();
		tempResolution = PlayerPrefs.GetString ("resolution", originalResolution).Split("x"[0]);
		resolution = new Vector2 (int.Parse(tempResolution [0]), int.Parse(tempResolution [1]));
//		Debug.Log ("Resoltion: " + resolution);

		vsync = GetBool ("vsync", false);
		aa = PlayerPrefs.GetInt ("aa", 1);

		isFullscreen = GetBool ("fullscreen", true);
	}

	void GetGS () {
		displayNameTags = GetBool ("displayNameTags", true);
		displayNoobHints = GetBool ("displayNoobHints", true);
	}

	public void UpdateSettings () {
		sfxVolume = PlayerPrefs.GetFloat ("sfxVolume", 0f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", -12f);
        displayFPS = GetBool("displayFPS", false);
		motionBlur = GetBool ("motionBlur", true);
		bloom = GetBool ("bloom", true);
		chromaticAberration = GetBool ("ca", true);
        GetSS ();
		GetGS ();

		ChangeSettings ();
	}

	void ChangeSettings () {
		/*if (Application.platform == RuntimePlatform.OSXEditor)
			return;*/

		// Debug.Log ("Changing settings...");

		StartCoroutine (Fullscreen (isFullscreen));
		if (vsync) {
			QualitySettings.vSyncCount = 1;
		} else {
			QualitySettings.vSyncCount = 0;
		}
		QualitySettings.antiAliasing = aa;
        Camera.main.GetComponent<CameraMotionBlur>().enabled = motionBlur;
        Camera.main.GetComponent<BloomOptimized>().enabled = bloom;
        Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = chromaticAberration;

        mixer.SetFloat ("SFXMixVolume", sfxVolume);
		mixer.SetFloat ("MusicMixVolume", musicVolume);

		fpsDisplay.SetActive (displayFPS);
	}

	private IEnumerator Fullscreen (bool fullscreen) {
		int width = Mathf.FloorToInt (resolution.x);
		int height = Mathf.FloorToInt (resolution.y);
		Screen.fullScreen = fullscreen;

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		Screen.SetResolution (width, height, Screen.fullScreen);
	}
}
