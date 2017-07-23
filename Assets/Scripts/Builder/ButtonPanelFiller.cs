using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttachmentTypes { Cannons, Shields, Mines, Hulls, Specials }

public class ButtonPanelFiller : MonoBehaviour {

	public AttachmentTypes selectedType;
	private Object[] attachments;

	// Use this for initialization
	void Start () {
		attachments = Resources.LoadAll("Attachments/Resources");
		List<GameObject> tilesToAdd = new List<GameObject>();

		foreach (GameObject go in attachments) {
			if (selectedType == AttachmentTypes.Cannons) {
				if (go.name.Contains("C_")) {
					tilesToAdd.Add(go);
				}
			} else if (selectedType == AttachmentTypes.Shields) {
				if (go.name.Contains("S_")) {
					tilesToAdd.Add(go);
				}
			} else if (selectedType == AttachmentTypes.Mines) {
				if (go.name.Contains("M_")) {
					tilesToAdd.Add(go);
				}
			} else if (selectedType == AttachmentTypes.Hulls) {
				if (go.name.Contains("H_")) {
					tilesToAdd.Add(go);
				}
			} else if (selectedType == AttachmentTypes.Specials) {
				if (go.name.Contains("SP_")) {
					tilesToAdd.Add(go);
				}
			}
		}

		foreach (GameObject go in tilesToAdd) {
			GameObject tile = (GameObject)Instantiate(Resources.Load("Attachments/BuilderTile"));
			tile.transform.SetParent(transform);
			tile.transform.localScale = Vector3.one;
			tile.transform.Find("Image").GetComponent<Image>().sprite = go.GetComponent<SpriteRenderer>().sprite;
			tile.transform.Find("Image").GetComponent<BuilderTile>().attachment = go;
			tile.transform.Find("Image").GetComponent<BuilderTile>().pr = GameObject.Find("PixelsRemaining").GetComponent<PixelsRemaining>();
			tile.transform.Find("Price").GetComponent<PriceTag>().LoadPrice();
			tile.name = go.name + "Tile";
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
