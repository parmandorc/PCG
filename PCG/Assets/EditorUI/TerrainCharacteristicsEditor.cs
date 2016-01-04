using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TerrainCharacteristicsEditor : MonoBehaviour {

	// References to the sliders the editor has
	public Slider averageHeightSlider;
	public Slider flatnessSlider;
	public Slider roughnessSlider;

	public MaterialEditor materialEditor;

	// The window which all area list elements are attached to
	public GameObject areaListWindow;

	// The minimap object
	public Minimap minimap;

	// Data structures for managing the buttons for all the terrain areas
	public List<GameObject> terrainAreaButtons;
	private Dictionary<Color, TerrainAreaButton> terrainAreaButtonsByColor;

	// The prefab of the terrain area button on the area list
	public GameObject TerrainAreaButton_Prefab;

	// References to managers
	private TerrainCharacteristicsManager TCM;
	private ChunkLoader CL;

	// If true, changes in sliders will not affect the variables they control
	private bool dontAffectVariables = true;

	// The color key of Terrain Area that is being edited
	private Color selectedTerrainAreaColorKey;

	void Start() {
		//Dictionaries cant be initialized in the inspector. So we initialize it here with the values given in inspector for the list
		terrainAreaButtonsByColor = new Dictionary<Color, TerrainAreaButton> ();
		foreach (GameObject button in terrainAreaButtons) {
			TerrainAreaButton component = button.GetComponent<TerrainAreaButton>();
			terrainAreaButtonsByColor.Add(component.colorKey, component);
		}

		// Get references to managers
		TCM = TerrainCharacteristicsManager.Instance;
		CL = ChunkLoader.Instance;
		
		// Select the initial defaul area
		SetSelectedTerrainArea (Color.white, false);
	}

	public void ChangeAverageHeight(float value) {
		if (!dontAffectVariables) {
			TCM.getTerrainArea(selectedTerrainAreaColorKey).averageHeight = value;
			CL.ReloadAllChunks ();
		}
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

	public void ChangeMaterial(Eppy.Tuple<TerrainMaterial, Gradient> material) {
		if (!dontAffectVariables) {
			TCM.getTerrainArea(selectedTerrainAreaColorKey).material = material;
			CL.ReloadAllChunks ();
		}
	}

	public void SetSelectedTerrainArea(Color colorKey, bool deselect = true) {
		if (deselect)
			terrainAreaButtonsByColor [selectedTerrainAreaColorKey].Deselect ();

		selectedTerrainAreaColorKey = colorKey;

		//Initialize the sliders with the values of the selected area
		TerrainArea area = TCM.getTerrainArea (selectedTerrainAreaColorKey);
		dontAffectVariables = true;
		{
			averageHeightSlider.value = area.averageHeight;
			flatnessSlider.value = area.flatness;
			roughnessSlider.value = area.roughness;
			materialEditor.setToggleOn(area.material.Item1);
		}
		dontAffectVariables = false;

		//Make the minimap drawing use this color
		minimap.drawingColor = colorKey;
	}

	public void NewTerrainArea() {
		//Get a unique color for the new area
		Color colorKey = Color.white;
		while (TCM.colorKeyIsUsed(colorKey))
			colorKey = AuxiliarMethods.FixColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

		//Get the default name of the new area
		string name = "Terrain Area " + (terrainAreaButtons.Count + 1);

		//Create the TerrainArea object
		TCM.addTerrainArea (colorKey);

		//Create the new TerrainAreaButton
		GameObject terrainAreaButton = Instantiate (TerrainAreaButton_Prefab);
		TerrainAreaButton terrainAreaButtonComponent = terrainAreaButton.GetComponent<TerrainAreaButton> ();
		terrainAreaButton.transform.SetParent (areaListWindow.transform, false); //Set the parent to the ScrollView's Content window ; transform.parent = areaListWindow doesn't work
		terrainAreaButtonComponent.Init (colorKey, name, this);
		RectTransform lastTerrainAreaButtonRectTransform = terrainAreaButtons [terrainAreaButtons.Count - 1].GetComponent<RectTransform>();
		terrainAreaButton.GetComponent<RectTransform> ().position = lastTerrainAreaButtonRectTransform.position - new Vector3(0f, lastTerrainAreaButtonRectTransform.sizeDelta.y, 0f); //Set the position of the new element
		terrainAreaButtons.Add (terrainAreaButton);
		terrainAreaButtonsByColor.Add (colorKey, terrainAreaButtonComponent);
		areaListWindow.GetComponent<RectTransform> ().sizeDelta += new Vector2 (0f, lastTerrainAreaButtonRectTransform.sizeDelta.y); //Increse the content window's size

		//Set the new area as the selected one
		SetSelectedTerrainArea (colorKey);
	}

	public Gradient getColoringForMaterial(TerrainMaterial material) {
		return materialEditor.colorings[material];
	}
}
