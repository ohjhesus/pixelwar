using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class Builder : MonoBehaviour {

	public GameObject builderPanel;
	public GameObject builderPanelBackground;
	private GameObject localPlayer;

	public Material unlit;
	public PixelsRemaining pr;

	public float tileScaleAmount = 1.5f;
	public float snapDistance = 0.5f;

    private Material attachmentMat;

    private bool bloomWasOn;
	private bool chromicAberrationWasOn;

	private NetMgr netMgr;

    // Use this for initialization
    void Start () {
		netMgr = GameObject.Find("NetworkManager").GetComponent<NetMgr>();
		builderPanel.SetActive (false);
		builderPanelBackground.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		if (builderPanel.activeInHierarchy) {
			builderPanel.transform.parent.position = Camera.main.transform.position;
			builderPanelBackground.transform.parent.position = Camera.main.transform.position;
		}


		if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift)) {
			localPlayer = netMgr.localPlayer;
			if (localPlayer != null) {
				if (builderPanel.activeInHierarchy) {
					BuildPlayer();
				} else {
					OpenBuilder();
				}
			}
		}
	}

	void OpenBuilder () {
        builderPanel.SetActive(true);
		builderPanelBackground.SetActive(true);
		bloomWasOn = Camera.main.GetComponent<BloomOptimized>().enabled;
		Camera.main.GetComponent<BloomOptimized>().enabled = false;

		chromicAberrationWasOn = Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled;
		Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = false;
		pr.pixelsRemaining = localPlayer.GetComponent<Player> ().pixels;
		pr.UpdateCounter ();
		GetComponent<Pause> ().pausePanel.SetActive (false);
	}

    public void CloseBuilder()
    {
		Camera.main.GetComponent<BloomOptimized>().enabled = bloomWasOn;
		Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = chromicAberrationWasOn;
		builderPanel.SetActive(false);
		builderPanelBackground.SetActive(false);
	}

	public void BuildPlayer () {
		localPlayer = netMgr.localPlayer;

		Transform playermodel = GameObject.Find ("PlayerModel").transform;
		foreach (Transform bAttachment in playermodel) {
			if (bAttachment.tag == "Attachment") {
				localPlayer.GetComponent<Player> ().SpawnAttachment (bAttachment.name, bAttachment.localPosition.x / 140, bAttachment.localPosition.y / 140); //950
			}
		}

		localPlayer.GetComponent<Player> ().AffectHealth (pr.pixelsRemaining - localPlayer.GetComponent<Player> ().pixels);

		CloseBuilder ();
	}
}
