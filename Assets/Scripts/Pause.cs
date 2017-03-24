using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

    public GameObject pausePanel;

    // Use this for initialization
    void Start() {
		pausePanel.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (pausePanel.activeInHierarchy) {
				ClosePause ();
			} else {
				if (GetComponent<Builder> ().builderPanel.activeInHierarchy) {
					GetComponent<Builder> ().CloseBuilder ();
				} else {
					OpenPause ();
				}
			}
		}
  	}

	void OpenPause () {
		pausePanel.SetActive(true);
		GetComponent<Builder> ().builderPanel.SetActive (false);
	}

	void ClosePause () {
		pausePanel.SetActive(false);
	}
}