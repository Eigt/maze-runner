using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public float speed = 2f;

	private int hits = 0;

	// enemy movement is calculated here
	void FixedUpdate () {
		transform.position += transform.forward * Time.deltaTime * speed;
	}
		
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Room" || other.tag == "Floor") {
			return; // enter a room
		} else if (other.tag == "Cell") {
			// attempt to turn right upon entering a cell
			transform.Rotate (0, 90, 0);
			hits = 0;
		} else if (other.tag != "Projectile") {
			// turn left upon hitting a wall
			transform.Rotate (0, -90, 0);
			hits++;
			if (hits > 3) {
				// break out of the loop
				transform.Rotate (0, -90, 0);
			}
		}
		// because walls are actually two overlapping walls, the enemy will 180 upon hitting them. This is known and accounted for.
		// KNOWN ERROR: in corners of the maze, the enemy may glitch
	}
}
