using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TerrainCharacteristicsEditor : MonoBehaviour {

	public ChunkLoader cl;

	public Slider resolutionSlider;
	public Slider roughnessSlider;
	public Slider flatnessSlider;

	public void Init () {
		roughnessSlider.value = cl.persistence;
		flatnessSlider.value = cl.strength;
	}

	public void ChangeResolution(float value) {

	}

	public void ChangeRoughness(float value) {
		cl.persistence = value;
		cl.ReloadAllChunks ();
	}

	public void ChangeFlatness(float value) {
		cl.strength = value;
		cl.ReloadAllChunks ();
	}
}
