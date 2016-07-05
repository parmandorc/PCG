using UnityEngine;

//Used when the material coloring cannot be determined yet, and needs the data for interpolation, which will be done once the value has been calculated.
public class ExtendedTerrainArea : TerrainArea {

	private Gradient coloring00, coloring10, coloring01, coloring11;
	private float deltaX, deltaY;

	public ExtendedTerrainArea(float averageHeight, float flatness, float roughness, 
	                                  Gradient coloring00, Gradient coloring10, Gradient coloring01, Gradient coloring11,
	                                  float deltaX, float deltaY) : base(averageHeight, flatness, roughness, null) {
		this.coloring00 = coloring00;
		this.coloring10 = coloring10;
		this.coloring01 = coloring01;
		this.coloring11 = coloring11;
		this.deltaX = deltaX;
		this.deltaY = deltaY;
	}

	// Interpolates between the colors determined by the coloring of the different terrain areas
	public override Color evaluateColor(float value) {
		return Color.Lerp (Color.Lerp (coloring00.Evaluate(value), coloring10.Evaluate(value), deltaX),
		            Color.Lerp (coloring01.Evaluate(value), coloring11.Evaluate(value), deltaX),
		            deltaY);
	}
}
