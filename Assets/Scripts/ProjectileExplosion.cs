using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ProjectileExplosion : NetworkBehaviour {

	void OnEnable () {
		GetComponent<ParticleSystem> ().Emit(Random.Range(3, 6));
		Destroy (gameObject, GetComponent<ParticleSystem> ().startLifetime);
	}
}