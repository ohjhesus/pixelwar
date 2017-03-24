using UnityEngine;
using System.Collections;

public class AnimationDestroy : MonoBehaviour {

	public AnimationClip clip;

	// Use this for initialization
	void Start () {
		StartCoroutine (DestroyDelay ());
		StartCoroutine (LightFade ());
		transform.position = new Vector3 (transform.position.x, transform.position.y, -1);
	}

	IEnumerator DestroyDelay () {
		yield return new WaitForSeconds (clip.length);

		Destroy (gameObject);
	}

	IEnumerator LightFade () {
		yield return new WaitForSeconds (clip.length / 2);

		float startTime = Time.realtimeSinceStartup + clip.length / 2;
		float lerpTime = 0f;
		while (startTime > Time.realtimeSinceStartup) {
			lerpTime += 0.03f;
			GetComponent<Light> ().intensity = Mathf.Lerp (4, 0, lerpTime);
			yield return new WaitForSeconds (0);
		}
	}
}