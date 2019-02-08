using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 2f;
	public GameObject projectile;
	public Transform pSpawn;

	private bool shotActive;
	private bool gameWon;
	
	// player movement is calculated here
	void FixedUpdate () {
		if (gameWon) {
			return;
		}
		
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);

		movement.Set (moveHorizontal, 0f, moveVertical);
		movement = movement.normalized * speed * Time.deltaTime;
		transform.Translate (movement, Space.World);

		// change the direction the player object is facing
		if (Input.GetKey ("right")) {
			transform.eulerAngles = new Vector3 (0, 90, 0);
		} else if (Input.GetKey ("left")) {
			transform.eulerAngles = new Vector3 (0, -90, 0);
		} else if (Input.GetKey ("up")) {
			transform.eulerAngles = new Vector3 (0, 0, 0);
		} else if (Input.GetKey ("down")) {
			transform.eulerAngles = new Vector3 (0, 180, 0);
		}
	}

	// player shooting is handled here
	void Update () {
		if (gameWon) {
			return;
		}

		if (Input.GetKeyDown("space") && shotActive == false) {
			Instantiate(projectile, pSpawn.position, pSpawn.rotation);
			shotActive = true;
		}

	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Enemy") {
			// game over
			Destroy (other.gameObject);
			Destroy (this.gameObject);
		} else if (other.tag == "Key") {
			// pick up the key
			Destroy (other.gameObject);
		} else if (other.tag == "Room") {
			// spawn an enemy
			other.gameObject.GetComponent<RoomManager> ().spawnEnemy ();
		} else if (other.tag == "Goal") {
			// game won
			gameWon = true;
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.tag == "Cell") {
			// player ignores special cell colliders
			Physics.IgnoreCollision (collision.gameObject.GetComponent<Collider> (), GetComponent<Collider> ());
		} else if (collision.gameObject.name == "Entry Hall") {
			// render lower ground invisible while in maze
			GameObject[] ls = GameObject.FindGameObjectsWithTag ("Lower Ground");
			for (int i = 0; i < ls.Length; i++) {
				ls [i].GetComponent<Renderer> ().enabled = false;
			}
		} else if (collision.gameObject.name == "Exit Hall 2") {
			// render lower ground visible again once outside of maze
			GameObject[] ls = GameObject.FindGameObjectsWithTag ("Lower Ground");
			for (int i = 0; i < ls.Length; i++) {
				ls [i].GetComponent<Renderer> ().enabled = true;
			}
		} else if (collision.gameObject.name == "Patch") {
			// step closer to patch disappearance for maze access
			collision.gameObject.GetComponent<PatchManager> ().tick ();
		}
	}

	// shot destroyed is called by ProjectileMovement when the shot is destroyed
	public void shotDestroyed () {
		shotActive = false;
	}
}
