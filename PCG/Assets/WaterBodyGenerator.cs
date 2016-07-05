using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Parent class for all water body generation classes.
public abstract class WaterBodyGenerator : ThreadedJob {

	// Outputs
	public List<Vector3> output;

	// Inputs
	protected int resolution;
	protected int mapSize;
	protected Color[][] map;
	protected Dictionary<Color, TerrainArea> terrainAreas;
	protected TerrainArea optimizedTerrainArea;

	// Base constructor
	public WaterBodyGenerator(Vector2 origin) {
		this.resolution = TerrainChunk.resolution;
		this.mapSize = TerrainCharacteristicsManager.Instance.mapSize;
		this.terrainAreas = TerrainCharacteristicsManager.Instance.getTerrainAreasDeepCopy();
		object map = TerrainCharacteristicsManager.Instance.getChunkMap (new Vector2 (-1, -1));
		if (map is Color[][])
			this.map = (Color[][])map;
		else if (map is Color) // Optimized if only one color
			this.optimizedTerrainArea = terrainAreas[(Color)map];
	}
		
	protected float getHeightValue(Vector2 coords) {
		TerrainArea parameters = this.optimizedTerrainArea;
		if (parameters == null)
			parameters = Noise.getParameters(coords.x / (resolution * mapSize), coords.y / (resolution * mapSize), map, terrainAreas);

		return Noise.FractalPerlin3D(new Vector3(coords.x / resolution - 0.5f, coords.y / resolution - 0.5f, 0f), 
			parameters.frequency, 6, parameters.lacunarity, parameters.roughness, parameters.flatness, parameters.averageHeight);
	}
}
