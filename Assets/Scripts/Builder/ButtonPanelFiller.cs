using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttachmentTypes { Cannons, Shields, Mines, Hulls, Specials }

public class ButtonPanelFiller : MonoBehaviour {

	public AttachmentTypes selectedType;

	// Use this for initialization
	void Start () {
		if (selectedType == AttachmentTypes.Cannons) {

		} else if (selectedType == AttachmentTypes.Shields) {

		} else if (selectedType == AttachmentTypes.Mines) {

		} else if (selectedType == AttachmentTypes.Hulls) {

		} else if (selectedType == AttachmentTypes.Specials) {

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
