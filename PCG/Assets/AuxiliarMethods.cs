using UnityEngine;
//using System.Collections;

public static class AuxiliarMethods {
	
	public static float sqr2 = Mathf.Sqrt(2f);
	
	public static float Smooth (float t) {
		return t * t * t * (t * (t * 6f - 15f) + 10f);
	}
	
	public static float Dot (Vector2 g, float x, float y) {
		return g.x * x + g.y * y;
	}
	
	public static float Dot (Vector3 g, float x, float y, float z) {
		return g.x * x + g.y * y + g.z * z;
	}

	public static Color FixColor(Color color) {
		return new Color (Mathf.Round(color.r * 10f) / 10f, Mathf.RoundToInt(color.g * 10f) / 10f, Mathf.RoundToInt(color.b * 10f) / 10f);
	}
}
