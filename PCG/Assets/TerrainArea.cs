using UnityEngine;

// Class responsible for holding the defining data of a terrain area
public class TerrainArea {

	// Controls initial scale.
	public float frequency = 1f;

	// Controls how frequency changes in every fractal octave
	public float lacunarity = 2f;

	// Controls the vertical offset
	public float averageHeight;

	// Controls the vertical scale
	public float flatness;

	// Controls the persistence of the noise, that is, how amplitude changes in every fractal octave.
	// This means that lower values will give the initial octave more intensity, hence creating smooth terrain,
	//		whereas higher values will give the last octaves more intensity, hence creating rougher terrain.
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

	public string ToJson() {
		return "{"+
			"\"frequency\":"+frequency.ToString()+","+
			"\"lacunarity\":"+lacunarity.ToString()+","+
			"\"averageHeight\":"+averageHeight.ToString()+","+
			"\"flatness\":"+flatness.ToString()+","+
			"\"roughness\":"+roughness.ToString()+
			"}";
	}
}
