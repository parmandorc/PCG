using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Class responsible for the selection of a terrain area in the multizone editor.
public class TerrainAreaButton : MonoBehaviour {

	//Colors for selected and deselected states
	public Color deselectedColor;
	public Color selectedColor;

	//The color key of the element
	public Color colorKey;

	//References to other objects
	public Text nameTextComponent;
	public Button colorButtonComponent;
	public TerrainCharacteristicsEditor TCE;
	private Image imageComponent;

	public void OnClick() {
		TCE.SetSelectedTerrainArea (colorKey);

		imageComponent.color = selectedColor;
	}

	// Use this for initialization
	void Awake () {
		imageComponent = gameObject.GetComponent<Image> ();
	}

	// Initialization with parameters
	public void Init(Color colorKey, string name, TerrainCharacteristicsEditor TCE) {
		this.TCE = TCE;
		this.colorKey = colorKey;
		colorButtonComponent.GetComponent<Image> ().color = colorKey;
		nameTextComponent.text = name;

		//Terrain areas that are created are set to the selected one
		imageComponent.color = selectedColor;
	}

	public void Deselect() {
		imageComponent.color = deselectedColor;
	}
}
