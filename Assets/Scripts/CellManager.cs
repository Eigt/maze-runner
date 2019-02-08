using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour {

	private bool connected = false;
	private int x;
	private int z;

	// setX and setZ set the coordinates of the cell
	public void setX (int coord) {
		this.x = coord;
	}
	public void setZ (int coord) {
		this.z = coord;
	}
	// getX and getZ return the coordinates of the cell
	public int getX () {
		return this.x;
	}
	public int getZ () {
		return this.z;
	}
	// setStatus and getStatus set and return the connected status of the cell (for maze construction)
	public void setStatus (bool status) {
		this.connected = status;
	}
	public bool getStatus () {
		return this.connected;
	}

	// deleteWall removes a wall from the cell
	public void deleteWall (string wallName) {
		foreach (Transform child in transform) {
			if(wallName.CompareTo(child.name)==0){
				Destroy(child.gameObject);
			}
		}
	}
}
