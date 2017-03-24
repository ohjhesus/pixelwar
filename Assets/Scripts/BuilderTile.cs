using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuilderTile : MonoBehaviour {

	private bool isBeingDragged;
	private bool snappedToAnchor;
	private bool canGetClosestAnchor;
	private GameObject closestAnchor;
	private Vector3 closestAnchorLocalPos;
	private List<GameObject> anchors;
	private Vector3 startPos;
	private Vector2 oldMousePos;
	private Vector2 worldMousePos;

	public float scaleAmount;

	public Object attachment;
	private GameObject attachmentAsGO;
	private Sprite attachmentSprite;
	public PixelsRemaining pr;

	private Material attachmentMat;

	private Builder builder;

	// Use this for initialization
	void Start () {
		builder = GameObject.Find("GameControllers").GetComponent<Builder>();
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
		Vector2 position = Input.mousePosition;
		foreach (GameObject go in anchors) {
			Vector2 diff = Vector2.Distance (go.transform.position - new Vector3(position.x, position.y, 0));
			Debug.Log (go.name + diff.sqrMagnitude);
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}

		closestAnchorLocalPos = new Vector3(Screen.width / 2, (
				Screen.height + builder.builderPanel.transform.FindChild("BuildArea").GetComponent<RectTransform>().rect.yMin) / 2
			) + closest.GetComponent<RectTransform>().localPosition;
		return closest;
	}

	// Update is called once per frame
	void Update () {
		// Debug.Log (closestAnchorLocalPos);
		// Debug.Log (Input.mousePosition);

		worldMousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (!Input.mousePosition.Equals (oldMousePos)) {
			oldMousePos = Input.mousePosition;
			if (canGetClosestAnchor)
				closestAnchor = GetClosestAnchor ();
		}
			
		Debug.Log (Vector2.Distance (closestAnchorLocalPos, Input.mousePosition));
		if (Vector2.Distance (closestAnchorLocalPos, Input.mousePosition) < 50 && isBeingDragged) {
			snappedToAnchor = true;
			GetComponent<RectTransform> ().position = closestAnchor.GetComponent<RectTransform> ().position;//new Vector3 (closestAnchor.GetComponent<RectTransform>().position.x, -closestAnchorLocalPos.y, 0);// + new Vector3 (GetComponent<RectTransform> ().pivot.x, GetComponent<RectTransform>().pivot.y, 0);
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
						part.GetComponent<SpriteRenderer> ().sortingOrder = part.GetComponent<AttachToPlayer> ().sortingOrderDifference;
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