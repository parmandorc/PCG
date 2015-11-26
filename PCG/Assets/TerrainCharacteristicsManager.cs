using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainCharacteristicsManager : MonoBehaviour {

	// Reference to the editor
	public TerrainCharacteristicsEditor TCE;

	// Default coloring of the terrain
	public Gradient defaultColoring;

	// Default characterstics of new areas
	public float defaultAverageHeight, defaultFlatness, defaultRoughness;

	// Static singleton property
	public static TerrainCharacteristicsManager Instance { get ; private set; }

	// Table of the terrain areas that form the map (key = color, value = terrainArea)
	private Dictionary<Color, TerrainArea> terrainAreas;

	// The resolution of the maps in chunks
	private int mapSize = 5;

	// The pixel resolution a chunk has in the map image
	private const int chunkSize = 100;

	void Awake () {
		// First we check if there are any other instances conflicting
		if(Instance != null && Instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}
		
		// Here we save our singleton instance
		Instance = this;

		// Create default terrain area
		terrainAreas = new Dictionary<Color, TerrainArea> ();
		terrainAreas.Add (Color.white, newDefaultTerrainArea());

		TCE.Init ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Returns a deepCopy of the terrainAreas table. Threads need a deep copy because values might change during the thread process.
	public Dictionary<Color, TerrainArea> getTerrainAreasDeepCopy() {
		Dictionary<Color, TerrainArea> deepCopy = new Dictionary<Color, TerrainArea> ();
		foreach (Color colorKey in terrainAreas.Keys) {
			deepCopy.Add(colorKey, (terrainAreas[colorKey]).deepCopy());
		}
		return deepCopy;
	}

	// Returns a reference to the requested TerrainArea
	public TerrainArea getTerrainArea(Color colorKey) {
		return terrainAreas[colorKey];
	}

	public bool colorKeyIsUsed(Color colorKey) {
		return terrainAreas.ContainsKey(colorKey);
	}

	public void addTerrainArea(Color colorKey) {
		terrainAreas.Add (colorKey, newDefaultTerrainArea ());
	}

	public TerrainArea newDefaultTerrainArea() {
		return new TerrainArea (defaultAverageHeight, defaultFlatness, defaultRoughness, defaultColoring);
	}
}
