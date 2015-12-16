using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ValueCalculationThreadedJob : ThreadedJob
{
	// Outputs
	public Vector3[] vertices;
	public Color[] colors;

	// Inputs
	private Vector2 chunkID;
	private int resolution;
	private Dictionary<Color, TerrainArea> terrainAreas;
	private Color[][] chunkMap;
	private TerrainArea optimizedTerrainArea = null;

	public ValueCalculationThreadedJob(Vector2 chunkID, int resolution, Vector3[] vertices, 
	                                   Dictionary<Color, TerrainArea> terrainAreas, Color[][] chunkMap) {
		this.chunkID = chunkID;
		this.resolution = resolution;
		this.vertices = vertices;
		this.terrainAreas = terrainAreas;
		this.chunkMap = chunkMap;
	}

	public ValueCalculationThreadedJob(Vector2 chunkID, int resolution, Vector3[] vertices, TerrainArea terrainArea) {
		this.chunkID = chunkID;
		this.resolution = resolution;
		this.vertices = vertices;
		this.optimizedTerrainArea = terrainArea;
	}

	protected override void OnRun() {
		if (optimizedTerrainArea != null)
			OptimizedCalculateValues ();
		else
			CalculateValues ();
	}

	// General algorith with interpolations
	private void CalculateValues() {
		Vector3 offset = new Vector3 (chunkID.x, chunkID.y, 0f);
		Vector3 point00 = new Vector3(-0.5f,-0.5f) + offset;
		Vector3 point10 = new Vector3( 0.5f,-0.5f) + offset;
		Vector3 point01 = new Vector3(-0.5f, 0.5f) + offset;
		Vector3 point11 = new Vector3( 0.5f, 0.5f) + offset;
		
		colors = new Color[vertices.Length];
		float stepSize = 1f / resolution;
		for (int v = 0, y = 0; y <= resolution; y++) {
			Vector3 point0 = Vector3.Lerp (point00, point01, y * stepSize);
			Vector3 point1 = Vector3.Lerp (point10, point11, y * stepSize);
			for (int x = 0; x <= resolution; x++, v++) {
				Vector3 point = Vector3.Lerp (point0, point1, x * stepSize);
				
				// Get parameters from terrain area
				Dictionary<string, object> parameters = getParameters((float)x / resolution, (float)y / resolution);
				
				// Calculate values
				float sample = Noise.Sum(point, (float)parameters["frequency"], 6, (float)parameters["lacunarity"], (float)parameters["persistence"]) + 0.5f;
				colors[v] = ((Gradient)parameters["coloring"]).Evaluate(sample);
				sample *= (float)parameters["strength"] / (float)parameters["frequency"];
				vertices[v].y = sample / 1;
			}
		}
	}

	// Optimized version of the algorithm, when only one terrainArea is to be processed.
	private void OptimizedCalculateValues() {
		Vector3 offset = new Vector3 (chunkID.x, chunkID.y, 0f);
		Vector3 point00 = new Vector3(-0.5f,-0.5f) + offset;
		Vector3 point10 = new Vector3( 0.5f,-0.5f) + offset;
		Vector3 point01 = new Vector3(-0.5f, 0.5f) + offset;
		Vector3 point11 = new Vector3( 0.5f, 0.5f) + offset;

		// Get parameters from terrain area
		float frequency = optimizedTerrainArea.frequency;
		float lacunarity = optimizedTerrainArea.lacunarity;
		float persistence = optimizedTerrainArea.roughness;
		float strength = optimizedTerrainArea.flatness;
		Gradient coloring = optimizedTerrainArea.coloring;

		colors = new Color[vertices.Length];
		float stepSize = 1f / resolution;
		for (int v = 0, y = 0; y <= resolution; y++) {
			Vector3 point0 = Vector3.Lerp (point00, point01, y * stepSize);
			Vector3 point1 = Vector3.Lerp (point10, point11, y * stepSize);
			for (int x = 0; x <= resolution; x++, v++) {
				Vector3 point = Vector3.Lerp (point0, point1, x * stepSize);
				
				// Calculate values
				float sample = Noise.Sum(point, frequency, 6, lacunarity, persistence) + 0.5f;
				colors[v] = coloring.Evaluate(sample);
				sample *= strength / frequency;
				vertices[v].y = sample / 1;
			}
		}
	}

	private Dictionary<string,object> getParameters(float x, float y) {
		Dictionary<string, object> parameters = new Dictionary<string, object> ();

		// Change coords from [0,1] scale to [0, chunkResolution]
		x = Mathf.Lerp (0, chunkMap.Length - 2, x);
		y = Mathf.Lerp (0, chunkMap [0].Length - 2, y);

		// Get coords for the terrain area for interpolation
		int xCoord = Mathf.FloorToInt(x - 0.5f) + 1;
		int yCoord = Mathf.FloorToInt(y - 0.5f) + 1;

		// Get color keys for the 4 areas
		Color colorKey00 = chunkMap[xCoord][yCoord];
		Color colorKey10 = chunkMap[xCoord + 1][yCoord];
		Color colorKey01 = chunkMap[xCoord][yCoord + 1];
		Color colorKey11 = chunkMap[xCoord + 1][yCoord + 1];

		if (!terrainAreas.ContainsKey (colorKey00) || !terrainAreas.ContainsKey (colorKey01) || 
		    !terrainAreas.ContainsKey (colorKey10) || !terrainAreas.ContainsKey (colorKey11))
			Debug.Log ("EXCEPTION: Color key not existent!");

		// Optimization: 4 areas are the same
		if (colorKey00 == colorKey01 && colorKey00 == colorKey10 && colorKey00 == colorKey11) {
			TerrainArea terrainArea = terrainAreas [colorKey00];
			parameters.Add ("frequency", terrainArea.frequency);
			parameters.Add ("lacunarity", terrainArea.lacunarity);
			parameters.Add ("persistence", terrainArea.roughness);
			parameters.Add ("strength", terrainArea.flatness);
			parameters.Add ("coloring", terrainArea.coloring);

		} else {

			// Get the 4 areas
			TerrainArea terrainArea00 = terrainAreas [colorKey00];
			TerrainArea terrainArea10 = terrainAreas [colorKey10];
			TerrainArea terrainArea01 = terrainAreas [colorKey01];
			TerrainArea terrainArea11 = terrainAreas [colorKey11];

			// Get interpolated parameters
			float deltaX = x - xCoord + 0.5f;
			float deltaY = y - yCoord + 0.5f;
			parameters.Add ("frequency", Mathf.Lerp (Mathf.Lerp (terrainArea00.frequency, terrainArea10.frequency, deltaX),
		                                        Mathf.Lerp (terrainArea01.frequency, terrainArea11.frequency, deltaX), deltaY));
			parameters.Add ("lacunarity", Mathf.Lerp (Mathf.Lerp (terrainArea00.lacunarity, terrainArea10.lacunarity, deltaX),
		                                         Mathf.Lerp (terrainArea01.lacunarity, terrainArea11.lacunarity, deltaX), deltaY));
			parameters.Add ("persistence", Mathf.Lerp (Mathf.Lerp (terrainArea00.roughness, terrainArea10.roughness, deltaX),
		                                          Mathf.Lerp (terrainArea01.roughness, terrainArea11.roughness, deltaX), deltaY));
			parameters.Add ("strength", Mathf.Lerp (Mathf.Lerp (terrainArea00.flatness, terrainArea10.flatness, deltaX),
		                                       Mathf.Lerp (terrainArea01.flatness, terrainArea11.flatness, deltaX), deltaY));
			parameters.Add ("coloring", terrainArea00.coloring);
		}

		return parameters;
	}
}
