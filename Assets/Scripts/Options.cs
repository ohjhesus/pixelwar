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

	public void Respawn (GameObject deathCanvas) {
		Debug.Log("respawn 1");
		GameObject.Find("NetworkManager").GetComponent<NetMgr>().Respawn();
		deathCanvas.SetActive(false);
	}

	void Update () {
		if ((Input.GetKey (KeyCode.LeftCommand) || Input.GetKey (KeyCode.LeftControl)) && Input.GetKeyDown (KeyCode.R)) {
			PlayerPrefs.SetString ("resolution", "1920x1080");
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
		sfxVolume = PlayerPrefs.GetFloat ("sfxVolume", 0.6f);
		musicVolume = PlayerPrefs.GetFloat ("musicVolume", 0.6f);
		displayFPS = GetBool("displayFPS", true);
		motionBlur = GetBool ("motionBlur", false);
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

		// music / sfx volume
		float sfxDB = 0f;
		float musicDB = 0f;
		if (sfxVolume != 0) {
			sfxDB = 20.0f * Mathf.Log10 (sfxVolume);
		} else {
			sfxDB = -144.0f;
		}
		if (musicVolume != 0) {
			musicDB = 20.0f * Mathf.Log10 (musicVolume);
		} else {
			musicDB = -144.0f;
		}
		mixer.SetFloat ("SFXMixVolume", sfxDB);
		mixer.SetFloat ("MusicMixVolume", musicDB);

		fpsDisplay.SetActive (displayFPS);

		UpdateOptionsUI ();
	}

	void UpdateOptionsUI () {
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("OptionsSubmenu")) {
			foreach (Transform submenuChild in go.transform) {
				if (submenuChild.GetComponent<Toggle> ()) {
					submenuChild.GetComponent<Toggle> ().isOn = GetBool (submenuChild.GetComponent<UIInteractions> ().ppVar);
				} else if (submenuChild.GetComponent<Slider> ()) {
					submenuChild.GetComponent<Slider> ().value = PlayerPrefs.GetFloat (submenuChild.GetComponent<UIInteractions> ().ppVar);
				} else if (submenuChild.GetComponent<Dropdown> ()) {
					if (submenuChild.GetComponent<UIInteractions> ().ppVar == "resolution") {
//						submenuChild.GetComponent<Dropdown> ().value = PlayerPrefs.GetString (submenuChild.GetComponent<UIInteractions> ().ppVar);
					} else {
						submenuChild.GetComponent<Dropdown> ().value = PlayerPrefs.GetInt (submenuChild.GetComponent<UIInteractions> ().ppVar);
					}
					submenuChild.GetComponent<Dropdown> ().RefreshShownValue ();
				} else {
					Debug.Log ("Unsupported object tried to update: " + submenuChild.name);
				}
			}
		}
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
