using UnityEngine;
using System.Collections;

public class TerrainArea {

	public float frequency = 1f;

	public float lacunarity = 2f;

	public float averageHeight;

	public float flatness;

	public float roughness;

	public Gradient coloring;

	public TerrainArea(float averageHeight, float flatness, float roughness, Gradient coloring) {
		this.averageHeight = averageHeight;
		this.flatness = flatness;
		this.roughness = roughness;
		this.coloring = coloring;
	}

	public TerrainArea deepCopy(){
		return new TerrainArea (averageHeight, flatness, roughness, coloring);
	}
}
