using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class responsible for the edition of defining data of water bodies and roads in the UI.
public class UpperLayersEditor : MonoBehaviour {

	public GameObject OceanUIElement_Prefab;

	public GameObject RiverUIElement_Prefab;

	public GameObject RoadUIElement_Prefab;

	public Transform OceansScrollview;

	public Transform RiversScrollview;

	public Transform RoadsScrollview;

	private List<WaterBodyUIElement> waterBodyUIElements;

	private List<RoadUIElement> roadUIElements;

	void Start() {
		waterBodyUIElements = new List<WaterBodyUIElement> ();
		roadUIElements = new List<RoadUIElement> ();
	}

	public void newOcean() {
		GameObject ocean = Instantiate (OceanUIElement_Prefab);
		ocean.transform.SetParent (OceansScrollview);
		WaterBodyUIElement uielement = ocean.GetComponent<WaterBodyUIElement> ();
		if (uielement != null) {
			uielement.Init (waterBodyUIElements.Count);
			waterBodyUIElements.Add (uielement);
			UpperLayersManager.Instance.newWaterBody (uielement.origin);
		}
	}

	public void newRiver() {
		GameObject river = Instantiate (RiverUIElement_Prefab);
		river.transform.SetParent (RiversScrollview);
		WaterBodyUIElement uielement = river.GetComponent<WaterBodyUIElement> ();
		if (uielement != null) {
			uielement.Init (waterBodyUIElements.Count, true);
			waterBodyUIElements.Add (uielement);
			UpperLayersManager.Instance.newWaterBody (uielement.origin);
		}
	}

	public void newRoad() {
		GameObject road = Instantiate (RoadUIElement_Prefab);
		road.transform.SetParent (RoadsScrollview);
		RoadUIElement uielement = road.GetComponent<RoadUIElement> ();
		if (uielement != null) {
			uielement.Init (roadUIElements.Count);
			roadUIElements.Add (uielement);
			UpperLayersManager.Instance.newRoad (uielement.origin, uielement.destination, uielement.maxSlope, uielement.bridgePenalty);
		}
	}

	public void setWaterBodyStatus(int index, UIElement.UIStatus status) {
		waterBodyUIElements [index].setStatus (status);
	}

	public void setRoadStatus(int index, UIElement.UIStatus status) {
		roadUIElements [index].setStatus (status);
	}
}
