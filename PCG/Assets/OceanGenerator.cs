using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class responsible for the flooding algorithm used in water masses generation
public class OceanGenerator : WaterBodyGenerator {

	// The height of the water level of the water mass
	private float height;

	// The open list container of the algorithm
	private List<Vector2> openList;

	// Duplicated structure of openList for time optimization, used for constant time contains-search.
	private HashSet<Vector2> openSet;

	// The closed list container of the algorithm. Again, it is a Set since we only need the contains-search operation, in constant time.
	private HashSet<Vector2> closedSet;

	// Base constructor
	public OceanGenerator(Vector2 origin, float height) : base(origin) {

		this.height = height;

		openList = new List<Vector2> ();
		openList.Add (new Vector2(Mathf.Round(origin.x * resolution), Mathf.Round(origin.y * resolution))); // Initialize open list with origin
		openSet = new HashSet<Vector2> (openList);
		closedSet = new HashSet<Vector2> ();
	}

	protected override void OnRun() {
		
		while (openList.Count > 0) {
			// Pop open list and switch element to closed list
			Vector2 current = openList [0];
			openList.RemoveAt(0);
			openSet.Remove (current);

			// Check if current point is inside map borders
			if (current.x >= 0 && current.x <= mapSize * resolution && current.y >= 0 && current.y <= mapSize * resolution && getHeightValue(current) < height) {

				closedSet.Add (current);

				// Obtain propagation points
				Vector2[] adyacents = {
					new Vector2 (current.x, current.y + 1),
					new Vector2 (current.x + 1, current.y),
					new Vector2 (current.x, current.y - 1),
					new Vector2 (current.x - 1, current.y)
				};

				// Add unvisited adyacent points to open list
				foreach (Vector2 ady in adyacents) {
					if (!closedSet.Contains (ady) && !openSet.Contains (ady)) {
						openList.Add (ady);
						openSet.Add (ady);
					}
				}
			}
		}

		output = new List<Vector3> ();
		foreach (Vector2 coord in closedSet) {
			output.Add (new Vector3 (coord.x / resolution, height, coord.y / resolution));
		}
	}
}
