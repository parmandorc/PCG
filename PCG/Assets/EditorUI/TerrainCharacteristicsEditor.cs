using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TerrainCharacteristicsEditor : MonoBehaviour {

	public TerrainCreator terrain;

	public Slider resolutionSlider;
	public Slider roughnessSlider;
	public Slider flatnessSlider;

	public void Init () {
		resolutionSlider.value = terrain.resolution;
		roughnessSlider.value = terrain.persistence;
		flatnessSlider.value = terrain.strength;
	}

	public void ChangeResolution(float value) {
		terrain.resolution = (int)value;
		terrain.Refresh ();
	}

	public void ChangeRoughness(float value) {
		terrain.persistence = value;
		terrain.Refresh ();
	}

	public void ChangeFlatness(float value) {
		terrain.strength = value;
		terrain.Refresh ();
	}
}
