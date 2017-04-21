using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	private GameObject player;
	public bool trackPlayer;

	private bool foundPlayer;
	private bool isShaking;

	private NetMgr netMgr;

	void Start () {
		netMgr = GameObject.FindGameObjectWithTag("NetMgr").GetComponent<NetMgr>();
		foundPlayer = false;
		isShaking = false;
	}

	void Update () {
		if (trackPlayer && !isShaking) {
			if (foundPlayer) {
				transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, transform.position.z);
			} else {
				FindPlayer();
			}
		}
	}

	void FindPlayer () {
		if (netMgr.localPlayer != null) {
			player = netMgr.localPlayer;
			foundPlayer = true;
		} else {
			foundPlayer = false;
		}
	}

	public IEnumerator LerpShake (float start, float end, float lerpTime) {
		isShaking = true;

		Vector3 startPos = transform.position;

		float currentLerpTime = Time.deltaTime;
		float shakeIntensity = start;
		while (currentLerpTime > Time.deltaTime - lerpTime) {
			if (trackPlayer) {
				startPos = new Vector3 (player.transform.position.x, player.transform.position.y, transform.position.z);
			}

			shakeIntensity = Mathf.Lerp (start, end, (Time.time - currentLerpTime) / lerpTime);
			transform.position = new Vector3 (startPos.x + Random.Range (-shakeIntensity, shakeIntensity), startPos.y + Random.Range (-shakeIntensity, shakeIntensity), transform.position.z);

			yield return new WaitForSeconds (0);
		}
	isShaking = false;
	}
}
