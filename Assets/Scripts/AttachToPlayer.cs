using UnityEngine;
using System.Collections;

public class AttachToPlayer : MonoBehaviour {

	public int price;
	public Transform target;
	public Vector3 localPos;
	public Object original;
	public bool sameColorAsShip;
	public int sortingOrderDifference = 1;

	// Use this for initialization
	void Start () {
		if (target != null) {
			target.GetComponent<Player> ().attachments.Add (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (target == null)
			return;

		transform.localPosition = localPos;
	}
}