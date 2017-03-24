using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuilderTile : MonoBehaviour {

	private bool isBeingDragged;
	private bool snappedToAnchor;
	private bool canGetClosestAnchor;
	private GameObject closestAnchor;
	private List<GameObject> anchors;
	private Vector3 startPos;
	private Vector2 oldMousePos;
	private Vector2 worldMousePos;

	private float scaleAmount;

	public Object attachment;
	private GameObject attachmentAsGO;
	private Sprite attachmentSprite;
	public PixelsRemaining pr;

	private Material attachmentMat;

	private Builder builder;

	// Use this for initialization
	void Start () {
		builder = GameObject.Find("GameControllers").GetComponent<Builder>();
		scaleAmount = builder.tileScaleAmount;
		anchors = new List<GameObject> ();
		oldMousePos = Input.mousePosition;
		UpdateAnchors ();
		closestAnchor = GetClosestAnchor ();
		attachmentMat = builder.unlit;
		attachmentAsGO = (GameObject)attachment;
		attachmentSprite = attachmentAsGO.GetComponent<SpriteRenderer>().sprite;
	}

	void UpdateAnchors () {
		canGetClosestAnchor = false;
		anchors.Clear ();
		foreach (GameObject go in GameObject.FindGameObjectsWithTag ("Anchor")) {
			anchors.Add (go);
		}
		canGetClosestAnchor = true;
	}

	GameObject GetClosestAnchor () {
		UpdateAnchors ();

		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector2 position = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		foreach (GameObject go in anchors) {
			float diff = Vector2.Distance (go.transform.position, position);
			//Debug.Log (go.name + diff);
			if (diff < distance) {
				closest = go;
				distance = diff;
			}
		}

		return closest;
	}

	// Update is called once per frame
	void Update () {
		worldMousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (!Input.mousePosition.Equals (oldMousePos)) {
			oldMousePos = Input.mousePosition;
			if (canGetClosestAnchor)
				closestAnchor = GetClosestAnchor ();
		}
			
		//Debug.Log (Vector2.Distance (closestAnchor.transform.position, worldMousePos));
		if (Vector2.Distance (closestAnchor.transform.position, worldMousePos) < 0.8 && isBeingDragged) {
			snappedToAnchor = true;
			GetComponent<RectTransform>().position = closestAnchor.GetComponent<RectTransform>().position;
		} else {
			snappedToAnchor = false;
		}

		if (Input.GetMouseButtonDown (0)) {
			if (Vector2.Distance (worldMousePos, transform.position) < 1) {
				isBeingDragged = true;
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			isBeingDragged = false;
			GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

			if (snappedToAnchor) {
				bool canPlace = true;
				foreach (GameObject att in GameObject.FindGameObjectsWithTag ("Attachment")) {
					if (Vector2.Distance (Camera.main.WorldToScreenPoint (att.transform.position), GetComponent<RectTransform> ().position) < 0.1f) {
						canPlace = false;
					}
				}

				foreach (GameObject att in GameObject.FindGameObjectsWithTag ("PreviousAttachment")) {
					if (Vector2.Distance (Camera.main.WorldToScreenPoint (att.transform.position), GetComponent<RectTransform> ().position) < 0.1f) {
						canPlace = false;
					}
				}

				if (canPlace) {
					GameObject tempPart = (GameObject)attachment;

					if (pr.pixelsRemaining - tempPart.GetComponent<AttachToPlayer> ().price > 0) {
						GameObject part = (GameObject)Instantiate (attachment, transform.position, transform.rotation);
						Vector3 oldLocalScale = part.transform.localScale;
						part.transform.parent = GameObject.Find ("PlayerModel").transform;
						part.transform.localPosition = closestAnchor.transform.localPosition;
						part.tag = "Attachment";
						part.name = attachment.name;
						part.GetComponent<SpriteRenderer> ().sortingLayerName = "UI";
						part.layer = 5;
						part.GetComponent<SpriteRenderer> ().sortingOrder = part.GetComponent<AttachToPlayer> ().sortingOrder;
						part.transform.localScale = new Vector3 (oldLocalScale.x * 150f, oldLocalScale.y * 150f, 1);
						part.GetComponent<SpriteRenderer> ().material = attachmentMat;
						if (part.GetComponent<FollowMouse> ()) {
							part.GetComponent<FollowMouse> ().enabled = false;
						}

						closestAnchor.GetComponent<Image> ().enabled = false;

						pr.pixelsRemaining -= part.GetComponent<AttachToPlayer> ().price;
						pr.UpdateCounter ();
					}
				}

				snappedToAnchor = false;
			}

			GetComponent<RectTransform> ().localPosition = new Vector2 (0, 0);
			GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		}

		if (isBeingDragged) {
			if (!snappedToAnchor) {
				transform.position = worldMousePos;
			}
			GetComponent<RectTransform> ().localScale = new Vector3 (scaleAmount, scaleAmount, 1);
			GetComponent<RectTransform>().pivot = new Vector2 (attachmentSprite.pivot.x / attachmentSprite.texture.width, attachmentSprite.pivot.y / attachmentSprite.texture.width);
		}
	}
}