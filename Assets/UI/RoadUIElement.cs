using UnityEngine;
using System.Collections;

// Class responsible for the edition of a road element in the UI
public class RoadUIElement : UIElement {

	public Vector2 origin { get; private set; }

	public Vector2 destination { get; private set; }

	public float maxSlope { get; private set; }

	public float bridgePenalty { get; private set; }

	private int index;

	private Transform originBeacon;

	private Transform destinationBeacon;

	public void Init(int index) {
		this.index = index;
		origin = new Vector2 (-1f, -1f);
		destination = new Vector2 (-1f, -1f);
		maxSlope = 1f;
		bridgePenalty = 1f;

		originBeacon = Instantiate (Beacon_Prefab).transform;
		originBeacon.position = new Vector3(origin.x, -1f, origin.y);
		destinationBeacon = Instantiate (Beacon_Prefab).transform;
		destinationBeacon.position = new Vector3 (destination.x, -1f, destination.y);
	}

	public void setOriginX(string value) {
		float x = float.Parse (value);
		origin = new Vector3 (x, origin.y);
		originBeacon.position = new Vector3(origin.x, -1f, origin.y);
		UpperLayersManager.Instance.changeRoad (index, origin, destination, maxSlope, bridgePenalty);
	}

	public void setDestinationX(string value) {
		float x = float.Parse (value);
		destination = new Vector3 (x, destination.y);
		destinationBeacon.position = new Vector3(destination.x, -1f, destination.y);
		UpperLayersManager.Instance.changeRoad (index, origin, destination, maxSlope, bridgePenalty);
	}

	public void setOriginY(string value) {
		float y = float.Parse (value);
		origin = new Vector3 (origin.x, y);
		originBeacon.position = new Vector3(origin.x, -1f, origin.y);
		UpperLayersManager.Instance.changeRoad (index, origin, destination, maxSlope, bridgePenalty);
	}

	public void setDestinationY(string value) {
		float y = float.Parse (value);
		destination = new Vector3 (destination.x, y);
		destinationBeacon.position = new Vector3(destination.x, -1f, destination.y);
		UpperLayersManager.Instance.changeRoad (index, origin, destination, maxSlope, bridgePenalty);
	}

	public void setMaxSlope(float value) {
		maxSlope = Mathf.Tan(value * Mathf.PI / 2f);
		UpperLayersManager.Instance.changeRoad (index, origin, destination, maxSlope, bridgePenalty);
	}

	public void setBridgePenalty(float value) {
		float threshold = 0.75f;
		if (value <= threshold)
			bridgePenalty = Mathf.Lerp (1f, 1.1f, value / 0.75f);
		else
			bridgePenalty = Mathf.Pow (Mathf.Lerp (1f, 1.25f, (value - 0.75f)/0.25f), 10f) + 0.1f;
		UpperLayersManager.Instance.changeRoad (index, origin, destination, maxSlope, bridgePenalty);
	}
}
