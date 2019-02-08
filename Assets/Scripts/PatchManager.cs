using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchManager : MonoBehaviour {

	public Material firstMat;
	public Material secondMat;

	public int count;
	
	public void tick () {
		count++;
		if (count == 1) {
			GetComponent<Renderer>().material = firstMat;
		} else if (count == 2) {
			GetComponent<Renderer>().material = secondMat;
		} else if (count >= 3) {
			Destroy (this.gameObject);
		}
	}
}
