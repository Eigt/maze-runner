using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManager : MonoBehaviour {

	private int count = 3;

	// update counts the number of keys left in the maze
	void Update () {
		count = GameObject.FindGameObjectsWithTag ("Key").Length;
		if (count <= 0) {
			// remove the gate when there are no more keys
			Destroy(this.gameObject);
		}
	}
}
