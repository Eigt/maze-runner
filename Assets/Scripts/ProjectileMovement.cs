using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour {

	private float pSpeed;
	private PlayerController pc;

	// Awake is called upon instantiation of the projectile
	void Awake() {
		pc = GameObject.Find ("Player").GetComponent<PlayerController> ();
		pSpeed = pc.speed * 2;
	}

	// FixedUpdate handles the projectiles movement
	void FixedUpdate () {
		transform.position += transform.forward * Time.deltaTime * pSpeed;
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Room" || other.tag == "Cell") {
			// projectile ignores special colliders
			return;
		} else if (other.tag != "Enemy") {
			// projectile destroyed upon hitting a wall
			Destroy (this.gameObject);
			pc.shotDestroyed ();
		} else {
			if (Random.value >= 0.25) {
				// 75% chance of destroying enemy
				Destroy (this.gameObject);
				Destroy (other.gameObject);
				pc.shotDestroyed ();
			}

		}
	}
}
