using UnityEngine;

// Class containing several auxiliar and mathematical methods for use in the application
public static class AuxiliarMethods {

	public static float sqr2 = Mathf.Sqrt(2f);

	// Applies the smoothing function t^5 - 15t^4 + 10t^3.
	// This 5th level polynom was chosen by Ken Perlin due to its first and second derivative evaluating to 0 for values of t of 0 and 1 
	// 		(the limit values of the interpolation coefficients that the function is applied to).
	public static float Smooth (float t) {
		return t * t * t * (t * (t * 6f - 15f) + 10f);
	}

	// Rounds a color. This avoids problems where a color in the minimap texture might be slightly different from the color key in the data structures.
	public static Color FixColor(Color color) {
		return new Color (Mathf.Round(color.r * 10f) / 10f, Mathf.RoundToInt(color.g * 10f) / 10f, Mathf.RoundToInt(color.b * 10f) / 10f);
	}
}
