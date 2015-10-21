using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float movementSpeed = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 moveVector = new Vector3 (Input.GetAxis ("Horizontal"), 0f, Input.GetAxis ("Vertical")).normalized;
		if (moveVector.sqrMagnitude > 0.001) {
			transform.position += moveVector * movementSpeed * Time.deltaTime;
		}

		ChunkLoader.Instance.UpdateTick (this);
	}
}
