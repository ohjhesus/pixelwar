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
		//localPlayer = GameObject.Find("player1");
		pr.pixelsRemaining = localPlayer.GetComponent<Player> ().pixels;
		pr.UpdateCounter ();
		GetComponent<Pause> ().pausePanel.SetActive (false);

		foreach (GameObject go in localPlayer.GetComponent<Player> ().attachments) {
			Object attachment = go.GetComponent<AttachToPlayer> ().original;
			GameObject part = (GameObject)Instantiate (attachment, transform.position, transform.rotation);
			Vector3 oldLocalScale = part.transform.localScale;
			part.transform.parent = GameObject.Find ("PlayerModel").transform;
			part.transform.localPosition = go.GetComponent<AttachToPlayer> ().localPos * 950;
			part.tag = "PreviousAttachment";
			part.name = attachment.name;
			part.transform.localScale = new Vector3 (oldLocalScale.x * 150, oldLocalScale.y * 150, 1);
			part.layer = 5;
			part.GetComponent<SpriteRenderer> ().sortingOrder = part.GetComponent<AttachToPlayer> ().sortingOrder;
			part.GetComponent<SpriteRenderer> ().sortingLayerName = "UI";

			if (part.GetComponent<FollowMouse> ()) {
				part.GetComponent<FollowMouse> ().enabled = false;
			}
				
			if (part.GetComponent<AttachToPlayer> ().aboveOrBelow == "above") {
				part.GetComponent<SpriteRenderer> ().sortingOrder += 100;
			} else if (part.GetComponent<AttachToPlayer> ().aboveOrBelow == "below") {
				part.GetComponent<SpriteRenderer> ().sortingOrder -= 100;
			}

			part.transform.localScale = new Vector3 (oldLocalScale.x * 150f, oldLocalScale.y * 150f, 1);
			part.GetComponent<SpriteRenderer> ().material = unlit;

			foreach (GameObject anchor in GameObject.FindGameObjectsWithTag ("Anchor")) {
				if (new Vector2 (transform.position.x, transform.position.y) == new Vector2 (anchor.transform.position.x, anchor.transform.position.y)) {
					anchor.GetComponent<Image> ().enabled = false;
				}
			}
		}
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
		/*foreach (GameObject go in GameObject.FindGameObjectsWithTag ("Attachment")) {
			if (go.GetComponent<AttachToPlayer> ().target = localPlayer.transform) {
				if (go.GetComponent<Shoot> ()) {
					go.GetComponent<Shoot> ().RemoveFromLists ();
				}

				Destroy (go);
			}
		}*/

		Transform playermodel = GameObject.Find ("PlayerModel").transform;
		foreach (Transform bAttachment in playermodel) {
			if (bAttachment.tag == "Attachment") {
				localPlayer.GetComponent<Player> ().SpawnAttachment (bAttachment.name, bAttachment.localPosition.x / 950, bAttachment.localPosition.y / 950); //950
			}
		}

		localPlayer.GetComponent<Player> ().AffectHealth (pr.pixelsRemaining - localPlayer.GetComponent<Player> ().pixels);

		CloseBuilder ();
	}
}
