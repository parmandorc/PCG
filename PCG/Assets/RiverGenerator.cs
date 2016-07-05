using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class responsible for the flooding algorithm used in water courses generation
public class RiverGenerator : WaterBodyGenerator {

	// Container for the open list that allows constant time access of the lowest point.
	private SortedList<float, List<Vector2>> openList;

	// Duplicated container for open list used for constant time contains-search.
	private HashSet<Vector2> openSet;

	// Duplicated container for the closed list, with constant time contains-search.
	// Note: the inherited property 'output' acts as the actual closed list.
	private HashSet<Vector2> closedSet;

	// Base constructor.
	public RiverGenerator(Vector2 origin) : base(origin) {
	
		origin = new Vector2 (Mathf.Round (origin.x * resolution), Mathf.Round (origin.y * resolution));

		openList = new SortedList<float, List<Vector2>>();
		openSet = new HashSet<Vector2> ();
		openList.Add (getHeightValue (origin), new List<Vector2>() {origin});
		openSet.Add (origin);

		output = new List<Vector3> ();
		closedSet = new HashSet<Vector2> ();
	}

	protected override void OnRun() {

		// Used to allow the algorithm to keep iterating over minimum flat surfaces after the rivers final point has been reached
		float finalExpansionHeight = -1f;

		while (openList.Count > 0) {
			// Pop open list
			Vector2 current = openList.Values [0][0];
			float height = openList.Keys [0];
			openList.Values [0].RemoveAt (0);
			if (openList.Values [0].Count == 0) {
				openList.RemoveAt (0);
			}
			openSet.Remove (current);

			// Check if point is inside map borders
			if (current.x >= 0 && current.x <= mapSize * resolution && current.y >= 0 && current.y <= mapSize * resolution &&
				(finalExpansionHeight == -1f || height <= finalExpansionHeight)) {

				output.Add (new Vector3 (current.x / resolution, height, current.y / resolution));
				closedSet.Add (current);
				for (int i = 0; i < output.Count; i++) { // Create local minimum lake
					if (output [i].y < height) {
						output [i] = new Vector3 (output [i].x, height, output [i].z);
					}
				}

				Vector2[] adyacents = {
					new Vector2 (current.x, current.y + 1),
					new Vector2 (current.x + 1, current.y),
					new Vector2 (current.x, current.y - 1),
					new Vector2 (current.x - 1, current.y)
				};

				foreach (Vector2 ady in adyacents) {
					if (!closedSet.Contains (ady) && !openSet.Contains (ady)) {
						float newHeight = getHeightValue (ady);
						if (!openList.ContainsKey (newHeight))
							openList.Add (newHeight, new List<Vector2> ());
						openList[newHeight].Add(ady);
						openSet.Add (ady);
					}
				}
			} else if (finalExpansionHeight == -1f){
				finalExpansionHeight = height;
			}
		}
	}
}
