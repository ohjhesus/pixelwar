using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : Photon.MonoBehaviour {

	private Object[] aiObjects;

	public float initialDelay = 100f;
	public float minCooldown;
	public float maxCooldown;
	public float cooldownPerPlayerMultiplier = 1.0f;

	private Vector3 spawnPos;
	private bool canSpawn = false;

	// Use this for initialization
	void Start () {
		if (PhotonNetwork.isMasterClient) {
			aiObjects = Resources.LoadAll("EnemyAI/Resources");
			canSpawn = false;
			StartCoroutine(StartDelay());
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (canSpawn) {
			canSpawn = false;
			StartCoroutine (SpawnAI ());
			StartCoroutine (Cooldown ());
		}
	}

	void OnMasterClientSwitched(PhotonPlayer newMasterClient) {
		Restart();
	}

	public void Restart () {
		if (PhotonNetwork.isMasterClient) {
			aiObjects = Resources.LoadAll("EnemyAI/Resources");
			canSpawn = false;
			StartCoroutine(Cooldown());
		}
	}

	IEnumerator SpawnAI () {
		int canSpawnCount = 0;
		spawnPos = new Vector2(Random.Range (-50f, 50f), Random.Range (-50f, 50f));
		while (canSpawnCount != PhotonNetwork.playerList.Length) { // while canSpawnCount != number of players
			canSpawnCount = 0;
			spawnPos = new Vector2(Random.Range (-50f, 50f), Random.Range (-50f, 50f)); // randomise SO spawn point

			foreach (GameObject player in GameObject.FindGameObjectsWithTag ("Player")) { // for each player object:
				if (Vector2.Distance (spawnPos, new Vector2 (player.transform.position.x, player.transform.position.y)) > 10) { // if spawn point is outside player view
					canSpawnCount++; // increase canSpawnCount by 1
				}
			}
			yield return new WaitForSeconds (0);
		}

		int index = Random.Range (0, aiObjects.Length); // pick random SO
		//GameObject spaceObject = (GameObject)PhotonNetwork.Instantiate (spaceObjects [index].name, spawnPos, Quaternion.identity, 0);
		GameObject enemyAI = (GameObject)PhotonNetwork.InstantiateSceneObject(aiObjects[index].name, spawnPos, Quaternion.identity, 0, null); // create SO object
		EnemyAI AIScript = enemyAI.GetComponent<EnemyAI>(); // load script from SO
	}

	IEnumerator Cooldown () {
		yield return new WaitForSeconds (Random.Range (minCooldown, maxCooldown) / (cooldownPerPlayerMultiplier * PhotonNetwork.playerList.Length));
		//		yield return new WaitForSeconds (1);
		canSpawn = true;
	}

	IEnumerator StartDelay () {
		yield return new WaitForSeconds (initialDelay);
		canSpawn = true;
	}
}
