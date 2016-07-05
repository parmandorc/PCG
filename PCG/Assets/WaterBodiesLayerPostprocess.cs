using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class responsible for the postprocess of the water bodies.
// This is used as time optimization for road generation, in order to have quicker access to the height value of a point that belongs to a water body,
//		since this gives constant time access as oppossed to the complexity of searching in a list of lists.
public class WaterBodiesLayerPostprocess : ThreadedJob {

	//Output
	public Dictionary<Vector2, float> result;

	//Input
	private List<List<Vector3>> waterBodies;

	public WaterBodiesLayerPostprocess() {
		result = new Dictionary<Vector2, float> ();
		waterBodies = UpperLayersManager.Instance.getWaterBodies ();
	}

	protected override void OnRun() {
		foreach (List<Vector3> waterBody in waterBodies) {
			foreach (Vector3 point in waterBody) {
				Vector2 p = new Vector2 (point.x, point.z);
				if (!result.ContainsKey (p) || result[p] < point.y)
					result [p] = point.y;
			}
		}
	}
}
