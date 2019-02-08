using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : CellManager {

	public GameObject enemy;

	private bool enemySpawned = false;
	private int doorX;
	private int doorZ;
	private int dir;
	private float offset = 0.5f;

	// spawnEnemy creates an enemy when the player first enters the room
	public void spawnEnemy () {
		if (enemySpawned == true) {
			return;
		}
		enemySpawned = true;
		Vector3 position = new Vector3 (0, 0, 0);
		Quaternion rotation = Quaternion.identity;
		if (dir == 1) {
			// door to the left
			position = new Vector3 (getX()+3, 0, getZ()+doorZ+offset);
			rotation = Quaternion.Euler (0, -90, 0);
		} else if (dir == 2) {
			// door above
			position = new Vector3 (getX()+doorX+offset, 0, getZ()+3);
			rotation = Quaternion.Euler (0, 0, 0);
		} else if (dir == 3) {
			// door to the right
			position = new Vector3 (getX()+3, 0, getZ()+doorZ+offset);
			rotation = Quaternion.Euler (0, 90, 0);
		} else if (dir == 4) {
			// door below
			position = new Vector3 (getX()+doorX+offset, 0, getZ()+3);
			rotation = Quaternion.Euler (0, 180, 0);
		}
	
		Instantiate (enemy, position, rotation);
	}

	public void setDoorX (int coord) {
		doorX = coord;
	}
	public void setDoorZ (int coord) {
		doorZ = coord;
	}
	public void setDir (int direction) {
		dir = direction;
	}

}
