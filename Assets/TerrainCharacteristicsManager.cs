using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Singleton class responsible for the control of the terrain areas and their defining data.
public class TerrainCharacteristicsManager : MonoBehaviour {

	// Reference to the Editor
	public TerrainCharacteristicsEditor TCE;

	// Reference to the Minimap
	public Minimap minimap;

	// Default coloring of the terrain
	public TerrainMaterial defaultMaterial;

	// Default characterstics of new areas
	public float defaultAverageHeight, defaultFlatness, defaultRoughness;

	// Static singleton property
	public static TerrainCharacteristicsManager Instance { get ; private set; }

	// The resolution of the map in chunks
	public int mapSize = 5;

	// Table of the terrain areas that form the map (key = color, value = terrainArea)
	private Dictionary<Color, TerrainArea> terrainAreas;


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
	}

	// Returns a deepCopy of the terrainAreas table. Threads need a deep copy because values might change during the thread process.
	public Dictionary<Color, TerrainArea> getTerrainAreasDeepCopy() {
		Dictionary<Color, TerrainArea> deepCopy = new Dictionary<Color, TerrainArea> ();
		foreach (Color colorKey in terrainAreas.Keys) {
			deepCopy.Add(colorKey, (terrainAreas[colorKey]).deepCopy());
		}
		return deepCopy;
	}

	// Returns a copy of the portion of the minimap texture corresponding to the given chunk.
	// The map is extended a pixel in every direction for the interpolation for graduality.
	public object getChunkMap(Vector2 chunkID) {
		int chunkResolution = minimap.chunkResolution;
		if (chunkID == new Vector2(-1f, -1f)) {
			chunkResolution *= mapSize;
			chunkID = new Vector2 (0f, 0f);
		}

		Color[][] chunkMap = new Color[chunkResolution + 2][];
		for (int i = 0; i < chunkResolution + 2; i++)
			chunkMap [i] = new Color[chunkResolution + 2];

		// Optimization if map only contains one color
		bool optimizable = true;
		Color? optimizedColorKey = null;

		int xOffset = (int)chunkID.x * chunkResolution;
		int yOffset = (int)chunkID.y * chunkResolution;
		for (int x = -1; x < chunkResolution + 1; x++) {
			for (int y = -1; y < chunkResolution + 1; y++) {
				Color color = AuxiliarMethods.FixColor(minimap.getMinimapTexturePixel(x + xOffset, y + yOffset));
				chunkMap[x + 1][y + 1] = color;

				// Optimization if map only contains one color
				if (optimizable) {
					if (optimizedColorKey == null)
						optimizedColorKey = color;
					else if (optimizedColorKey != color)
						optimizable = false;
				}
			}
		}

		return (optimizable) ? (object)optimizedColorKey : (object)chunkMap;
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
		return new TerrainArea (defaultAverageHeight, defaultFlatness, defaultRoughness, new Eppy.Tuple<TerrainMaterial, Gradient>(defaultMaterial, TCE.getColoringForMaterial(defaultMaterial)));
	}

	public string SaveDataToJson() {
		string output = "{";

		// Save mapSize
		output += "\"mapSize\":" + mapSize.ToString () + ",";

		// Save terrain areas
		output += "\"terrainAreas\":[";
		foreach (TerrainArea area in terrainAreas.Values) {
			output += area.ToJson() + ",";
		}
		output = output.Remove(output.Length - 1);
		output += "],";

		// Save minimap
		output += "\"areasMapMatrix\":" + minimap.ToJson(new List<Color>(terrainAreas.Keys));

		output += "}";
		return output;
	}
}
