using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public int gridSize = 16;
	public int roomSize = 4;
	public GameObject room;
	public GameObject cell;

	private int x1, x2, x3;
	private int z1, z2, z3;
	private GameObject[] roomArray;
	private GameObject[][] cellArray;
	List<Node> frontier = new List<Node>();

	// initialization
	void Start () {
		roomArray = new GameObject[3];
		cellArray = new GameObject[16][];
		for (int i=0; i<gridSize; i++) {
			cellArray[i] = new GameObject[gridSize];
		}
		createRooms ();
		createCells ();
		createMaze ();
	}

	// places the three rooms containing keys
	void createRooms() {
		// choose random coordinates for first room
		x1 = Random.Range(0, gridSize-roomSize);
		z1 = Random.Range(0, gridSize-roomSize);
		// the room cannot occupy (0,0)
		if (x1 == 0 && z1 == 0) {
			x1++;
			z1++;
		}
		Vector3 firstPos = new Vector3 (x1, 0, z1);
		roomArray[0] = Instantiate (room, firstPos, Quaternion.identity);
		roomArray [0].GetComponent<RoomManager> ().setX (x1);
		roomArray [0].GetComponent<RoomManager> ().setZ (z1);

		// repeat for second room
		x2 = Random.Range(0, gridSize-roomSize-1) +1;
		z2 = Random.Range(0, gridSize-roomSize-1) +1;
		// cannot overlap with first room
		while (x2 > x1 - roomSize && x2 < x1 + roomSize && z2 > z1 - roomSize && z2 < z1 + roomSize) {
			x2 = Random.Range(0, gridSize-roomSize-1) +1;
			z2 = Random.Range(0, gridSize-roomSize-1) +1;
		}
		Vector3 secondPos = new Vector3 (x2, 0, z2); 
		roomArray[1] = Instantiate (room, secondPos, Quaternion.identity);
		roomArray [1].GetComponent<RoomManager> ().setX (x2);
		roomArray [1].GetComponent<RoomManager> ().setZ (z2);

		// repeat for third room
		x3 = Random.Range(0, gridSize-roomSize-1) +1;
		z3 = Random.Range(0, gridSize-roomSize-1) +1;
		// cannot overlap with first or second room
		while ((x3 > x1 - roomSize && x3 < x1 + roomSize && z3 > z1 - roomSize && z3 < z1 + roomSize) || (x3 > x2 - roomSize && x3 < x2 + roomSize && z3 > z2 - roomSize && z3 < z2 + roomSize)) {
				x3 = Random.Range(0, gridSize-roomSize-1) +1;
				z3 = Random.Range(0, gridSize-roomSize-1) +1;
		}
		Vector3 thirdPos = new Vector3 (x3, 0, z3); 
		roomArray[2] = Instantiate (room, thirdPos, Quaternion.identity);
		roomArray [2].GetComponent<RoomManager> ().setX (x3);
		roomArray [2].GetComponent<RoomManager> ().setZ (z3);

	}

	// places cells to fill the remaining space
	void createCells () {
		for (int i = 0; i < gridSize; i++) {
			for (int j = 0; j < gridSize; j++) {
				// skip creation at coordinates if a room occupies the space
				if (i >= x1 && i < x1 + roomSize && j >= z1 && j < z1 + roomSize) {
					continue;
				} else if (i >= x2 && i < x2 + roomSize && j >= z2 && j < z2 + roomSize) {
					continue;
				} else if (i >= x3 && i < x3 + roomSize && j >= z3 && j < z3 + roomSize) {
					continue;
				}
				// else create a cell at the coordinates
				Vector3 position = new Vector3 (i, 0, j);
				cellArray[i][j] = Instantiate(cell, position, Quaternion.identity);
				cellArray [i] [j].GetComponent<CellManager> ().setX (i);
				cellArray [i] [j].GetComponent<CellManager> ().setZ (j);
			}
		}
	}

	// createMaze generates a maze and connects the relevant cells
	void createMaze () {
		// connect first cell to entry
		cellArray[0][0].GetComponent<CellManager>().deleteWall("South Wall");
		cellArray [0] [0].GetComponent<CellManager> ().setStatus (true);
		// connect last cell to exit
		cellArray[15][15].GetComponent<CellManager>().deleteWall("North Wall");

		addAdjacent (0, 0);
		// keep connecting and adding cells until the frontier is empty
		while (frontier.Count > 0) {
			int rand = Random.Range (0, frontier.Count);
			GameObject randChild = frontier[rand].node;
			GameObject randParent = frontier[rand].parent;
			if (frontier [rand].isRoom == true) {
				connectToRoom (randParent, randChild);
			} else {
				connectCells (randParent, randChild);
				addAdjacent (randChild.GetComponent<CellManager> ().getX (), randChild.GetComponent<CellManager> ().getZ ());
			}
			frontier.RemoveAt(rand);
		}
	}

	// connectCells receives two cells and deletes their common wall
	void connectCells (GameObject o1, GameObject o2) {
		CellManager cell1 = o1.GetComponent<CellManager> ();
		CellManager cell2 = o2.GetComponent<CellManager> ();

		int x1 = cell1.getX ();
		int z1 = cell1.getZ ();
		int x2 = cell2.getX ();
		int z2 = cell2.getZ ();

		if (x1 - x2 == 1 && z1 - z2 == 0) {
			// cell1 is to the right
			cell1.deleteWall("West Wall");
			cell2.deleteWall("East Wall");
		} else if (x2 - x1 == 1 && z1 - z2 == 0) {
			// cell1 is to the left
			cell1.deleteWall("East Wall");
			cell2.deleteWall("West Wall");
		} else if (x1 - x2 == 0 && z1 - z2 == 1) {
			// cell1 is above
			cell1.deleteWall("South Wall");
			cell2.deleteWall("North Wall");
		} else if (x1 - x2 == 0 && z2 - z1 == 1) {
			// cell1 is below
			cell1.deleteWall("North Wall");
			cell2.deleteWall("South Wall");
		} else {
			return;
		}
	}

	// connectToRoom receives a cell and a room, and deletes their common wall
	void connectToRoom (GameObject cell, GameObject room) {
		RoomManager manager = room.GetComponent<RoomManager> ();
		int roomX = manager.getX ();
		int roomZ = manager.getZ ();
		int cellX = cell.GetComponent<CellManager> ().getX ();
		int cellZ = cell.GetComponent<CellManager> ().getZ ();

		// find then delete the common wall
		if (cellX + 1 == roomX) {
			// cell is to the left
			manager.setDoorX (0);
			manager.setDir (1);
			cellArray [cellX] [cellZ].GetComponent<CellManager> ().deleteWall ("East Wall");
			if (cellZ == roomZ) {
				manager.setDoorZ (0);
				manager.deleteWall ("West Wall 1");
			} else if (cellZ - 1 == roomZ) {
				manager.setDoorZ (1);
				manager.deleteWall ("West Wall 2");
			} else if (cellZ - 2 == roomZ) {
				manager.setDoorZ (2);
				manager.deleteWall ("West Wall 3");
			} else if (cellZ - 3 == roomZ) {
				manager.setDoorZ (3);
				manager.deleteWall ("West Wall 4");
			}
		} else if (cellX - 4 == roomX) {
			// cell is to the right
			manager.setDoorX (3);
			manager.setDir (3);
			cellArray [cellX] [cellZ].GetComponent<CellManager> ().deleteWall ("West Wall");
			if (cellZ == roomZ) {
				manager.setDoorZ (0);
				manager.deleteWall ("East Wall 4");
			} else if (cellZ - 1 == roomZ) {
				manager.setDoorZ (1);
				manager.deleteWall ("East Wall 3");
			} else if (cellZ - 2 == roomZ) {
				manager.setDoorZ (2);
				manager.deleteWall ("East Wall 2");
			} else if (cellZ - 3 == roomZ) {
				manager.setDoorZ (3);
				manager.deleteWall ("East Wall 1");
			}
		} else if (cellZ + 1 == roomZ) {
			// cell is below
			manager.setDoorZ (0);
			manager.setDir (4);
			cellArray [cellX] [cellZ].GetComponent<CellManager> ().deleteWall ("North Wall");
			if (cellX == roomX) {
				manager.setDoorX (0);
				manager.deleteWall ("South Wall 4");
			} else if (cellX - 1 == roomX) {
				manager.setDoorX (1);
				manager.deleteWall ("South Wall 3");
			} else if (cellX - 2 == roomX) {
				manager.setDoorX (2);
				manager.deleteWall ("South Wall 2");
			} else if (cellX - 3 == roomX) {
				manager.setDoorX (3);
				manager.deleteWall ("South Wall 1");
			}
		} else if (cellZ - 4 == roomZ) {
			// cell is above
			manager.setDoorZ (3);
			manager.setDir (2);
			cellArray [cellX] [cellZ].GetComponent<CellManager> ().deleteWall ("South Wall");
			if (cellX == roomX) {
				manager.setDoorX (0);
				manager.deleteWall ("North Wall 1");
			} else if (cellX - 1 == roomX) {
				manager.setDoorX (1);
				manager.deleteWall ("North Wall 2");
			} else if (cellX - 2 == roomX) {
				manager.setDoorX (2);
				manager.deleteWall ("North Wall 3");
			} else if (cellX - 3 == roomX) {
				manager.setDoorX (3);
				manager.deleteWall ("North Wall 4");
			}
		}
	}

	// addAdjacent adds all unconnected cells adjacent to coordinates to the frontier array
	void addAdjacent (int x, int z) {
		// try to add the cell above
		if (z < 15) {
			if (cellArray [x] [z + 1] != null) {
				if (cellArray [x] [z + 1].GetComponent<CellManager> ().getStatus () == false) {
					Node child = new Node (cellArray [x] [z], cellArray [x] [z + 1]);
					frontier.Add (child);
					cellArray [x] [z + 1].GetComponent<CellManager> ().setStatus (true);
				}
			} else {
				GameObject room = roomArray[0];
				// find which room is adjacent
				for (int i = 0; i < roomArray.Length; i++) {
					int roomX = roomArray [i].GetComponent<RoomManager> ().getX ();
					int roomZ = roomArray [i].GetComponent<RoomManager> ().getZ ();
					if (x >= roomX && x < roomX + roomSize && z+1 == roomZ) {
						room = roomArray [i];
						break;
					}
				}
				if (room.GetComponent<RoomManager>().getStatus() == false) {
					Node child = new Node (cellArray[x][z], room, true);
					frontier.Add (child);
					room.GetComponent<RoomManager>().setStatus(true);
				}
			}
		}
		// try the cell to the right
		if (x < 15) {
			if (cellArray [x+1] [z] != null) {
				if (cellArray [x+1] [z].GetComponent<CellManager> ().getStatus () == false) {
					Node child = new Node (cellArray[x][z], cellArray[x+1][z]);
					frontier.Add (child);
					cellArray [x + 1] [z].GetComponent<CellManager> ().setStatus (true);
				}
			} else {
				GameObject room = roomArray[0];
				// find which room is adjacent
				for (int i = 0; i < roomArray.Length; i++) {
					int roomX = roomArray [i].GetComponent<RoomManager> ().getX ();
					int roomZ = roomArray [i].GetComponent<RoomManager> ().getZ ();
					if (x+1 == roomX && z >= roomZ && z < roomZ + roomSize) {
						room = roomArray [i];
						break;
					}
				}
				if (room.GetComponent<RoomManager>().getStatus() == false) {
					Node child = new Node (cellArray[x][z], room, true);
					frontier.Add (child);
					room.GetComponent<RoomManager>().setStatus(true);
				}
			}
		}
		// try the cell below
		if (z > 0) {
			if (cellArray [x] [z - 1] != null) {
				if (cellArray [x] [z - 1].GetComponent<CellManager> ().getStatus () == false) {
					Node child = new Node (cellArray[x][z], cellArray[x][z-1]);
					frontier.Add (child);
					cellArray [x] [z - 1].GetComponent<CellManager> ().setStatus (true);
				}
			} else {
				GameObject room = roomArray[0];
				// find which room is adjacent
				for (int i = 0; i < roomArray.Length; i++) {
					int roomX = roomArray [i].GetComponent<RoomManager> ().getX ();
					int roomZ = roomArray [i].GetComponent<RoomManager> ().getZ ();
					if (x >= roomX && x < roomX + roomSize && z - 4  == roomZ) {
						room = roomArray [i];
						break;
					}
				}
				if (room.GetComponent<RoomManager>().getStatus() == false) {
					Node child = new Node (cellArray[x][z], room, true);
					frontier.Add (child);
					room.GetComponent<RoomManager>().setStatus(true);
				}
			}
		}
		// try the cell to the left
		if (x > 0) {
			if (cellArray [x-1] [z] != null) {
				if (cellArray [x-1] [z].GetComponent<CellManager> ().getStatus () == false) {
					Node child = new Node (cellArray[x][z], cellArray[x-1][z]);
					frontier.Add (child);
					cellArray [x-1] [z].GetComponent<CellManager> ().setStatus (true);
				}
			} else {
				GameObject room = roomArray[0];
				// find which room is adjacent
				for (int i = 0; i < roomArray.Length; i++) {
					int roomX = roomArray [i].GetComponent<RoomManager> ().getX ();
					int roomZ = roomArray [i].GetComponent<RoomManager> ().getZ ();
					if (x-4 == roomX && z >= roomZ && z < roomZ + roomSize) {
						room = roomArray [i];
						break;
					}
				}
				if (room.GetComponent<RoomManager>().getStatus() == false) {
					Node child = new Node (cellArray[x][z], room, true);
					frontier.Add (child);
					room.GetComponent<RoomManager>().setStatus(true);
				}
			}
		}
	}
		
}

// the Node struct includes a parent cell and the cell/room we are adding to the array
public struct Node {
	public GameObject parent;
	public GameObject node;
	public bool isRoom;

	// the node is usually a cell
	public Node (GameObject parentCell, GameObject currentCell) {
		parent = parentCell;
		node = currentCell;
		isRoom = false;
	}

	// constructor must explicitly state if it is a room
	public Node (GameObject parentCell, GameObject currentCell, bool status) {
		parent = parentCell;
		node = currentCell;
		isRoom = status;
	}
}
