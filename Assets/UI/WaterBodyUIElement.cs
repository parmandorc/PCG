using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Class responsible for the edition of a water body in the UI.
public class WaterBodyUIElement : UIElement {

	public Vector3 origin { get; private set; }

	private int index;

	private Transform beacon;

	public void Init(int index, bool isRiver = false) {
		this.index = index;
		origin = new Vector3 (-1, isRiver ? -1 : 0, -1);
		
		beacon = Instantiate (Beacon_Prefab).transform;
		beacon.position = origin;
	}

	public void setX(string value) {
		float x = float.Parse (value);
		origin = new Vector3 (x, origin.y, origin.z);
		beacon.position = origin;
		setStatus (UIStatus.Loading);
		UpperLayersManager.Instance.changeWaterBody (index, origin);
	}

	public void setY(string value) {
		float y = float.Parse (value);
		origin = new Vector3 (origin.x, origin.y, y);
		beacon.position = origin;
		UpperLayersManager.Instance.changeWaterBody (index, origin);
	}

	public void setHeight(float value) {
		origin = new Vector3 (origin.x, value, origin.z);
		beacon.position = origin;
		UpperLayersManager.Instance.changeWaterBody (index, origin);
	}
}
