using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TerrainCharacteristicsEditor : MonoBehaviour {

	public ChunkLoader cl;

	public Slider resolutionSlider;
	public Slider roughnessSlider;
	public Slider flatnessSlider;

	private bool initialized = false;

	public void Init () {
		roughnessSlider.value = cl.persistence;
		flatnessSlider.value = cl.strength;

		initialized = true;
	}

	public void ChangeResolution(float value) {

	}

	public void ChangeRoughness(float value) {
		if (initialized) {
			cl.persistence = value;
			cl.ReloadAllChunks ();
		}
	}

	public void ChangeFlatness(float value) {
		if (initialized) {
			cl.strength = value;
			cl.ReloadAllChunks ();
		}
	}
}
