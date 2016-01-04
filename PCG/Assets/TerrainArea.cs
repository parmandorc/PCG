using UnityEngine;
//using System.Collections;

public class TerrainArea {

	public float frequency = 1f;

	public float lacunarity = 2f;

	public float averageHeight;

	public float flatness;

	public float roughness;
	
	public Eppy.Tuple<TerrainMaterial,Gradient> material;
	
	public TerrainArea(float averageHeight, float flatness, float roughness, Eppy.Tuple<TerrainMaterial,Gradient> material) {
		this.averageHeight = averageHeight;
		this.flatness = flatness;
		this.roughness = roughness;
		this.material = material;
	}

	public TerrainArea deepCopy(){
		return new TerrainArea (averageHeight, flatness, roughness, new Eppy.Tuple<TerrainMaterial, Gradient>(material.Item1, material.Item2));
	}

	public virtual Color evaluateColor(float value) {
		return this.material.Item2.Evaluate (value);
	}
}
