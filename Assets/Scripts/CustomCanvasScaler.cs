using UnityEngine;
using System.Collections;

public class CustomCanvasScaler : MonoBehaviour {

	public int defaultWidth;
	public int defaultHeight;
	public float scaleFactor;
	
	public void ScaleCanvas () {
		int widthDifference = defaultWidth - Screen.currentResolution.width;
		GetComponent<RectTransform> ().localScale = new Vector3 (1 - (scaleFactor * widthDifference), 1 - (scaleFactor * widthDifference), 1 - (scaleFactor * widthDifference));
	}
}
