using UnityEngine;
using System.Collections;

// Class responsible for player navigation
public class PlayerController : MonoBehaviour {

	public float movementSpeed = 0.5f;

	// Use this for initialization
	void Start () {
		
		// Set player in the middle of the map
		float x, z;
		x = z = TerrainCharacteristicsManager.Instance.mapSize / 2f;
		transform.position = new Vector3(x, transform.position.y, z);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 moveVector = new Vector3 (Input.GetAxis ("Horizontal"), 0f, Input.GetAxis ("Vertical")).normalized;
		if (moveVector.sqrMagnitude > 0.001) {
			transform.position += moveVector * movementSpeed * Time.deltaTime;

			// Clamp position to map size
			int mapSize = TerrainCharacteristicsManager.Instance.mapSize;
			float x = Mathf.Clamp(transform.position.x, 0f, mapSize);
			float z = Mathf.Clamp(transform.position.z, -0.5f, mapSize - 0.5f);
			transform.position = new Vector3(x, transform.position.y, z);
		}
	}
}
