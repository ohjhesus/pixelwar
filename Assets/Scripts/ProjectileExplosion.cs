using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ProjectileExplosion : MonoBehaviour {
	public int minParticles;
	public int maxParticles;
	void OnEnable () {
		GetComponent<ParticleSystem> ().Emit(Random.Range(minParticles, maxParticles));
		Destroy (gameObject, GetComponent<ParticleSystem> ().main.startLifetime.constantMax);
	}
}
