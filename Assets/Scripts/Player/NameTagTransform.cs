using UnityEngine;
using System.Collections;

public class NameTagTransform : MonoBehaviour {

	[HideInInspector] public TextMesh text;

	// Use this for initialization
	void Start () {
		text = GetComponent<TextMesh>();

		text.text = transform.parent.name;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.parent.position + new Vector3 (-0.4f, 1.8f, 0f);
		transform.rotation = Quaternion.Euler (Vector3.zero);
	}
}
