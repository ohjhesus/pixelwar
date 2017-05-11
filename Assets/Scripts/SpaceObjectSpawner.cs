using UnityEngine;
using System.Collections;

public class SpaceObjectSpawner : Photon.MonoBehaviour {

	private Object[] spaceObjects;

	public float minCooldown;
	public float maxCooldown;
	public float cooldownPerPlayerMultiplier = 1.0f;

	private Vector3 spawnPos;
	private bool canSpawn = false;

	// Use this for initialization
	void Start () {
		if (PhotonNetwork.isMasterClient) {
			spaceObjects = Resources.LoadAll("SpaceObjects/Resources");
			canSpawn = false;
			StartCoroutine(Cooldown());
		}
	}

	void Update () {
		if (canSpawn) {
			canSpawn = false;
			StartCoroutine (SpawnSO ());
			StartCoroutine (Cooldown ());
		}
	}

	IEnumerator SpawnSO () {
		int canSpawnCount = 0;
		spawnPos = new Vector2(Random.Range (-50f, 50f), Random.Range (-50f, 50f));
		while (canSpawnCount != PhotonNetwork.playerList.Length) {
			canSpawnCount = 0;
			spawnPos = new Vector2(Random.Range (-50f, 50f), Random.Range (-50f, 50f));

			foreach (GameObject player in GameObject.FindGameObjectsWithTag ("Player")) {
				if (Vector2.Distance (spawnPos, new Vector2 (player.transform.position.x, player.transform.position.y)) > 10) {
					canSpawnCount++;
				}
			}
			yield return new WaitForSeconds (0);
		}

		int index = Random.Range (0, spaceObjects.Length);
		//GameObject spaceObject = (GameObject)PhotonNetwork.Instantiate (spaceObjects [index].name, spawnPos, Quaternion.identity, 0);
		GameObject spaceObject = (GameObject)PhotonNetwork.InstantiateSceneObject(spaceObjects[index].name, spawnPos, Quaternion.identity, 0, null);
		SpaceObject SOScript = spaceObject.GetComponent<SpaceObject>();

		Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		float size = Random.Range(SOScript.minSize, SOScript.maxSize);
		int health = Mathf.RoundToInt(size * 5f);
		float speed = Random.Range(SOScript.minSpeed, SOScript.maxSpeed);
		float rotSpeed = Random.Range(-1f, 1f);

		spaceObject.GetPhotonView().RPC("SetupSO", PhotonTargets.AllBufferedViaServer, direction, size, health, speed, rotSpeed);
	}

	IEnumerator Cooldown () {
		yield return new WaitForSeconds (Random.Range (minCooldown, maxCooldown) / (cooldownPerPlayerMultiplier * PhotonNetwork.playerList.Length));
//		yield return new WaitForSeconds (1);
		canSpawn = true;
	}
}
