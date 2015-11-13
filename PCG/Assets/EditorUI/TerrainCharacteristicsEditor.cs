using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TerrainCharacteristicsEditor : MonoBehaviour {

	// References to the sliders the editor has
	public Slider averageHeightSlider;
	public Slider flatnessSlider;
	public Slider roughnessSlider;

	// References to managers
	private TerrainCharacteristicsManager TCM;
	private ChunkLoader CL;

	// If true, changes in sliders will not affect the variables they control
	private bool dontAffectVariables = true;

	private Color selectedTerrainAreaColorKey;

	public void Init () {
		// Get references to managers
		TCM = TerrainCharacteristicsManager.Instance;
		CL = ChunkLoader.Instance;

		selectedTerrainAreaColorKey = Color.white;
		TerrainArea area = TCM.getTerrainArea (selectedTerrainAreaColorKey);

		// Initialize sliders with values from the just selected area
		dontAffectVariables = true;
		{
			averageHeightSlider.value = area.averageHeight;
			flatnessSlider.value = area.flatness;
			roughnessSlider.value = area.roughness;
		}
		dontAffectVariables = false;
	}

	public void ChangeFlatness(float value) {
		if (!dontAffectVariables) {
			TCM.getTerrainArea(selectedTerrainAreaColorKey).flatness = value;
			CL.ReloadAllChunks ();
		}
	}

	public void ChangeRoughness(float value) {
		if (!dontAffectVariables) {
			TCM.getTerrainArea(selectedTerrainAreaColorKey).roughness = value;
			CL.ReloadAllChunks ();
		}
	}


}
