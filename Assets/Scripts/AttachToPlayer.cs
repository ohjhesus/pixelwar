using UnityEngine;
using System.Collections;

public class AttachToPlayer : MonoBehaviour {

	public int price;
	public Transform target;
	public Vector3 localPos;
	public Object original;
	public bool sameColorAsShip;
	public int sortingOrder = 1;

	// Use this for initialization
	void Start () {
		if (target != null) {
			target.GetComponent<Player> ().attachments.Add (gameObject);
		}
	}
}