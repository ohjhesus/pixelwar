using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIInteractions : MonoBehaviour {

	public string ppVar;

	private Options options;

	// Use this for initialization
	void Start () {
		options = GameObject.Find ("GameManager").GetComponent<Options> ();

		if (GetComponent<Toggle> ()) {
			GetComponent<Toggle> ().isOn = options.GetBool (ppVar);
			GetComponent<Toggle> ().onValueChanged.AddListener (ChangePPBool);
		} else if (GetComponent<Button> ()) {
			//do nothing
		} else if (GetComponent<Dropdown> () && GetComponent<GetResolutions> ()) {
			GetComponent<Dropdown> ().onValueChanged.AddListener (ChangePPString);
		} else if (GetComponent<Dropdown> ()) {
			GetComponent<Dropdown> ().onValueChanged.AddListener (ChangePPInt);
			GetComponent<Dropdown> ().value = PlayerPrefs.GetInt (ppVar);
		} else if (GetComponent<Slider> ()) {
			GetComponent<Slider> ().onValueChanged.AddListener (ChangePPFloat);
			GetComponent<Slider> ().value = PlayerPrefs.GetFloat (ppVar);
		} else {
			Debug.LogError ("UI type not supported by UIInteractions!");
		}
	}

	public void DefaultRes () {
		foreach (string res in GetComponent<GetResolutions> ().resolutions) {
			if (res == PlayerPrefs.GetString (ppVar)) {
				GetComponent<Dropdown> ().value = GetComponent<GetResolutions> ().resolutions.IndexOf (res);
			}
		}
	}

	void ChangePPBool (bool b) {
		options.SetPPBool (ppVar, b);
		options.UpdateSettings ();
	}

	void ChangePPInt (int i) {
		options.SetPPInt (ppVar, i);
		options.UpdateSettings ();
	}

	void ChangePPFloat (float f) {
		options.SetPPFloat (ppVar, f);
		options.UpdateSettings ();
	}

	void ChangePPString (int i) {
		options.SetPPString (ppVar, GetComponent<GetResolutions> ().resolutions[i]);
		options.UpdateSettings ();
	}
}