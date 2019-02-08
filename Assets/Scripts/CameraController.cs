using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;

	private Vector3 offset;

	// initialization
	void Start () {
		offset = transform.position - player.transform.position;	
	}

	// LateUpdate is called once per frame BUT is guaranteed to run last
	void LateUpdate () {
		transform.position = player.transform.position + offset;
	}
}
