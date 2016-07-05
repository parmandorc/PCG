using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SurfaceCreator : MonoBehaviour {
	
	private Mesh mesh;

	[Range(1, 200)]
	public int resolution = 150;

	private int currentResolution;

	public float frequency = 3f;

	[Range(1,8)]
	public int octaves = 5;

	[Range(1f, 4f)]
	public float lacunarity = 2f;

	[Range(0f, 1f)]
	public float persistence = 0.5f;

	[Range(1, 3)]
	public int dimensions = 3;

	public NoiseMethodType type;

	public Gradient coloring;

	private Vector3[] vertices;
	private Vector3[] normals;
	private Color[] colors;

	public Vector3 offset;
	public Vector3 rotation;

	[Range(0f, 1f)]
	public float strength = 0.5f;

	public bool coloringForStrength;

	public bool damping;

	private void OnEnable () {
		if (mesh == null) {
			mesh = new Mesh ();
			mesh.name = "Surface Mesh";
			GetComponent<MeshFilter>().mesh = mesh;
		}
		Refresh ();
	}

	public void Refresh () {
		if (resolution != currentResolution) {
			CreateGrid ();
		}

		Quaternion q = Quaternion.Euler (rotation);
		Vector3 point00 = q * new Vector3(-0.5f,-0.5f) + offset;
		Vector3 point10 = q * new Vector3( 0.5f,-0.5f) + offset;
		Vector3 point01 = q * new Vector3(-0.5f, 0.5f) + offset;
		Vector3 point11 = q * new Vector3( 0.5f, 0.5f) + offset;

		NoiseMethod method = Noise.noiseMethods [(int)type] [dimensions - 1];
		float stepSize = 1f / resolution;
		float amplitude = damping ? strength / frequency : strength;
		for (int v = 0, y = 0; y <= resolution; y++) {
			Vector3 point0 = Vector3.Lerp (point00, point01, y * stepSize);
			Vector3 point1 = Vector3.Lerp (point10, point11, y * stepSize);
			for (int x = 0; x <= resolution; x++, v++) {
				Vector3 point = Vector3.Lerp (point0, point1, x * stepSize);
				float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
				sample = type == NoiseMethodType.Value ? (sample - 0.5f) : (sample * 0.5f);
				if (coloringForStrength) {
					colors[v] = coloring.Evaluate(sample + 0.5f);
					sample *= amplitude;
				}
				else {
					sample *= amplitude;
					colors[v] = coloring.Evaluate(sample + 0.5f);
				}
				vertices[v].y = sample;
			}
		}
		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.RecalculateNormals ();
	}

	public void CreateGrid () {
		currentResolution = resolution;
		mesh.Clear ();

		vertices = new Vector3[(resolution + 1) * (resolution + 1)];
		colors = new Color[vertices.Length];
		normals = new Vector3[vertices.Length];
		Vector2[] uv = new Vector2[vertices.Length];
		float stepSize = 1f / resolution;
		for (int v = 0, z = 0; z <= resolution; z++) {
			for (int x = 0; x <= resolution; x++, v++) {
				vertices[v] = new Vector3(x * stepSize - 0.5f, 0f, z * stepSize - 0.5f);
				colors[v] = Color.black;
				normals[v] = Vector3.up;
				uv[v] = new Vector2(x * stepSize, z * stepSize);
			}
		}
		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.normals = normals;
		mesh.uv = uv;

		int[] triangles = new int[resolution * resolution * 6];
		for (int t = 0, v = 0, y = 0; y < resolution; y++, v++) {
			for (int x = 0; x < resolution; x++, v++, t += 6) {
				triangles [t] = v;
				triangles [t + 1] = v + resolution + 1;
				triangles [t + 2] = v + 1;
				triangles [t + 3] = v + 1;
				triangles [t + 4] = v + resolution + 1;
				triangles [t + 5] = v + resolution + 2;
			}
		}
		mesh.triangles = triangles;
	}
}
