using UnityEngine;
using System.Collections;

public class SpaceObjectSpawner : MonoBehaviour {

	private Object[] spaceObjects;

	public float minCooldown;
	public float maxCooldown;

	private Vector3 spawnLocation;
	private bool canSpawn = false;

	// Use this for initialization
	void Start () {
		spaceObjects = Resources.LoadAll ("SpaceObjects/Resources");
		canSpawn = false;
		StartCoroutine (Cooldown ());
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
		spawnLocation = new Vector2(Random.Range (-50f, 50f), Random.Range (-50f, 50f));
		while (canSpawnCount != GameObject.FindGameObjectsWithTag ("Player").Length) {
			canSpawnCount = 0;
			spawnLocation = new Vector2(Random.Range (-50f, 50f), Random.Range (-50f, 50f));

			foreach (GameObject player in GameObject.FindGameObjectsWithTag ("Player")) {
				if (Vector2.Distance (spawnLocation, new Vector2 (player.transform.position.x, player.transform.position.y)) > 10) {
					canSpawnCount++;
				}
			}
			yield return new WaitForSeconds (0);
		}

		int index = Random.Range (0, spaceObjects.Length);
		GameObject spaceObject = (GameObject)Instantiate (spaceObjects [index], spawnLocation, Quaternion.identity);
		spaceObject.GetComponent<SpaceObject> ().direction = new Vector2 (Random.Range (-1f, 1f), Random.Range (-1f, 1f));
		spaceObject.tag = "SpaceObject";
		if (spaceObjects [index].name == "Asteroid") {
			spaceObject.GetComponent<SpaceObject> ().speed = Random.Range (10f, 50f);
			spaceObject.GetComponent<SpaceObject> ().size = Random.Range (0.5f, 2f);
		} else if (spaceObjects [index].name == "Comet") {
			spaceObject.GetComponent<SpaceObject> ().speed = Random.Range (30f, 80f);
			spaceObject.GetComponent<SpaceObject> ().size = Random.Range (0.5f, 1.5f);
		}
		spaceObject.GetComponent<SpaceObject> ().health = Mathf.RoundToInt (spaceObject.GetComponent<SpaceObject> ().size * 5f);
		spaceObject.GetComponent<SpaceObject> ().rotSpeed = Random.Range (-1f, 1f);

		//Log spaceObject stats
		Debug.Log ("spawning space object - size: " + spaceObject.GetComponent<SpaceObject> ().size + "; health: " + spaceObject.GetComponent<SpaceObject> ().health + "; speed: " + spaceObject.GetComponent<SpaceObject> ().speed + "; torque: " + spaceObject.GetComponent<SpaceObject> ().rotSpeed);
	}

	IEnumerator Cooldown () {
		yield return new WaitForSeconds (Random.Range (minCooldown, maxCooldown));
//		yield return new WaitForSeconds (1);
		canSpawn = true;
	}
}
