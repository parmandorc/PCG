using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum TerrainMaterial {
	GREEN_HILLS,
	MOUNTAINS,
	DESERT,
	MESA,
	SNOW_LANDS,
	OCEAN
};

public class MaterialEditor : MonoBehaviour {

	public Toggle greenHillsToggle, mountainsToggle, desertToggle, mesaToggle, snowLandsToggle, oceanToggle;

	public Gradient greenHillsColoring, mountainsColoring, desertColoring, mesaColoring, snowLandsColoring, oceanColoring;

	public TerrainCharacteristicsEditor TCE;

	public Dictionary<TerrainMaterial, Gradient> colorings;

	void Awake () {
		colorings = new Dictionary<TerrainMaterial, Gradient> ();
		colorings.Add (TerrainMaterial.GREEN_HILLS, greenHillsColoring);
		colorings.Add (TerrainMaterial.MOUNTAINS, mountainsColoring);
		colorings.Add (TerrainMaterial.DESERT, desertColoring);
		colorings.Add (TerrainMaterial.MESA, mesaColoring);
		colorings.Add (TerrainMaterial.SNOW_LANDS, snowLandsColoring);
		colorings.Add (TerrainMaterial.OCEAN, oceanColoring);
	}

	public void setGreenHills(bool value) {
		if (value) {
			setMaterial(TerrainMaterial.GREEN_HILLS);
		}
	}

	public void setMountains(bool value) {
		if (value) {
			setMaterial(TerrainMaterial.MOUNTAINS);
		}
	}

	public void setDesert(bool value) {
		if (value) {
			setMaterial(TerrainMaterial.DESERT);
		}
	}

	public void setMesa(bool value) {
		if (value) {
			setMaterial(TerrainMaterial.MESA);
		}
	}

	public void setSnowLands(bool value) {
		if (value) {
			setMaterial(TerrainMaterial.SNOW_LANDS);
		}
	}

	public void setOcean(bool value) {
		if (value) {
			setMaterial(TerrainMaterial.OCEAN);
		}
	}

	// Sets the corresponding toggle to On
	public void setToggleOn(TerrainMaterial material) {
		switch (material) {

		case TerrainMaterial.GREEN_HILLS:
			greenHillsToggle.isOn = true;
			break;

		case TerrainMaterial.MOUNTAINS:
			mountainsToggle.isOn = true;
			break;

		case TerrainMaterial.DESERT:
			desertToggle.isOn = true;
			break;

		case TerrainMaterial.MESA:
			mesaToggle.isOn = true;
			break;

		case TerrainMaterial.SNOW_LANDS:
			snowLandsToggle.isOn = true;
			break;

		case TerrainMaterial.OCEAN:
			oceanToggle.isOn = true;
			break;
		}
	}

	// Sends to the TCE the gradient of the material that was set
	public void setMaterial(TerrainMaterial material) {
		TCE.ChangeMaterial(new Eppy.Tuple<TerrainMaterial, Gradient>(material, colorings[material]));
	}
}
