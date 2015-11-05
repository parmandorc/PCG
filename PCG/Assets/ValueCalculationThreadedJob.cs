using UnityEngine;
using System.Collections;

public class ValueCalculationThreadedJob : ThreadedJob
{
	private Vector2 chunkID;
	private int resolution;
	private float frequency, lacunarity, persistence, strength;
	private Gradient coloring;

	public Vector3[] vertices;
	public Color[] colors;

	public ValueCalculationThreadedJob(Vector2 chunkID, int resolution, Vector3[] vertices,
	                                   float frequency, float lacunarity, float persistence, float strength, Gradient coloring) {
		this.chunkID = chunkID;
		this.resolution = resolution;
		this.vertices = vertices;
		this.frequency = frequency;
		this.lacunarity = lacunarity;
		this.persistence = persistence;
		this.strength = strength;
		this.coloring = coloring;
	}

	protected override void OnRun() {

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
				float sample = Noise.Sum(point, frequency, 6, lacunarity, persistence) + 0.5f;
				colors[v] = coloring.Evaluate(sample);
				sample *= strength / frequency;
				vertices[v].y = sample / 1;
			}
		}
	}
}
