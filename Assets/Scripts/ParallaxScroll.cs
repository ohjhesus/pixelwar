using UnityEngine;
using System.Collections;

public class ParallaxScroll : MonoBehaviour {

	public float scrollSpeed;
	
	void LateUpdate () {
		GetComponent<Renderer> ().material.SetTextureOffset ("_MainTex", new Vector2 (transform.position.x * (scrollSpeed / 10), transform.position.y * (scrollSpeed / 10)));
	}
}
