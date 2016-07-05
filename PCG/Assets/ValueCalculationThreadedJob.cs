using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class responsible for the generation of the terrain's orography of a specified terrain chunk.
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

	public ValueCalculationThreadedJob(Vector2 chunkID, Vector3[] vertices) {

		this.chunkID = chunkID;
		this.vertices = vertices;

		this.resolution = TerrainChunk.resolution;
		this.terrainAreas = TerrainCharacteristicsManager.Instance.getTerrainAreasDeepCopy();
		object chunkMap = TerrainCharacteristicsManager.Instance.getChunkMap (chunkID);
		if (chunkMap is Color[][])
			this.chunkMap = (Color[][])chunkMap;
		else if (chunkMap is Color) // Optimized if only one color
			this.optimizedTerrainArea = terrainAreas[(Color)chunkMap];
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
				TerrainArea parameters = Noise.getParameters((float)x / resolution, (float)y / resolution, chunkMap, terrainAreas);

				// Calculate values
				float sample = Noise.FractalPerlin3D(point, parameters.frequency, 6, parameters.lacunarity, parameters.roughness, parameters.flatness, parameters.averageHeight);
				colors[v] = parameters.evaluateColor(sample);
				vertices[v].y = sample;
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
		float averageHeight = optimizedTerrainArea.averageHeight;
		float frequency = optimizedTerrainArea.frequency;
		float lacunarity = optimizedTerrainArea.lacunarity;
		float persistence = optimizedTerrainArea.roughness;
		float strength = optimizedTerrainArea.flatness;
		Gradient coloring = optimizedTerrainArea.material.Item2;

		colors = new Color[vertices.Length];
		float stepSize = 1f / resolution;
		for (int v = 0, y = 0; y <= resolution; y++) {
			Vector3 point0 = Vector3.Lerp (point00, point01, y * stepSize);
			Vector3 point1 = Vector3.Lerp (point10, point11, y * stepSize);
			for (int x = 0; x <= resolution; x++, v++) {
				Vector3 point = Vector3.Lerp (point0, point1, x * stepSize);
				
				// Calculate values
				float sample = Noise.FractalPerlin3D(point, frequency, 6, lacunarity, persistence, strength, averageHeight);
				colors[v] = coloring.Evaluate(sample);
				vertices[v].y = sample;
			}
		}
	}

	private void ApplyUpperLayer(List<List<Vector3>> layer, Color color) {
		foreach (List<Vector3> points in layer) {
			foreach (Vector3 p in points) {
				Vector3 point = p - new Vector3 (chunkID.x, 0f, chunkID.y);
				if (point.x >= 0f && point.x <= 1f && point.z >= 0f && point.z <= 1f) {
					int v = Mathf.RoundToInt (point.x * resolution) + Mathf.RoundToInt (point.z * resolution) * (resolution + 1);
					if (vertices [v].y <= point.y) {
						vertices [v].y = point.y;
						colors [v] = color;
					}
				}
			}
		}
	}
}