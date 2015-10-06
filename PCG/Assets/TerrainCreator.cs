using UnityEngine;
//using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainCreator : MonoBehaviour {
	
	public int resolution = 128;

	public float frequency = 1f;
	
	[Range(1,8)]
	public int octaves = 5;
	
	[Range(1f, 4f)]
	public float lacunarity = 2f;
	
	[Range(0f, 1f)]
	public float persistence = 0.5f;

	public float strength = 1.0f;

	// The coloring for the terrain
	public Gradient coloring;
	
	public TerrainCharacteristicsEditor TCEditor;

	public Vector3 moveVector;

	public float zoomScale = 5f;

	private const float minZoomScale = 0.1f;

	private Vector3 offset;

	// The mesh of the generated terrain
	private Mesh terrainMesh;

	// The vertices of the terrain mesh
	private Vector3[] vertices;

	// The colors of the terrain mesh
	private Color[] colors;

	private int currentResolution;

	// Use this for initialization
	private void Start () {
		if (terrainMesh == null) {
			terrainMesh = new Mesh ();
			terrainMesh.name = "Terrain Mesh";
			GetComponent<MeshFilter>().mesh = terrainMesh;
		}

		TCEditor.Init ();

		offset = Vector3.zero;

		Refresh ();
	}

	// Update is called once per frame
	void Update () {

		moveVector = new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0f).normalized;

		float zoomDelta = Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime;

		if (moveVector.sqrMagnitude > 0.001 || Mathf.Abs(zoomDelta) > 0.001) {
			float newZoomScale = zoomScale * (1 - Mathf.Min(zoomDelta, 1f));
			zoomScale = Mathf.Max(newZoomScale, minZoomScale);
			offset += moveVector * zoomScale * Time.deltaTime * 0.5f;
			Refresh();
		}
	}

	// Refresh the terrain on the mesh
	public void Refresh() {

		if (resolution != currentResolution) {
			currentResolution = resolution;
			CreateGrid ();
		}

		Vector3 point00 = new Vector3(-0.5f,-0.5f) * zoomScale + offset;
		Vector3 point10 = new Vector3( 0.5f,-0.5f) * zoomScale + offset;
		Vector3 point01 = new Vector3(-0.5f, 0.5f) * zoomScale + offset;
		Vector3 point11 = new Vector3( 0.5f, 0.5f) * zoomScale + offset;
		
		float stepSize = 1f / resolution;
		for (int v = 0, y = 0; y <= resolution; y++) {
			Vector3 point0 = Vector3.Lerp (point00, point01, y * stepSize);
			Vector3 point1 = Vector3.Lerp (point10, point11, y * stepSize);
			for (int x = 0; x <= resolution; x++, v++) {
				Vector3 point = Vector3.Lerp (point0, point1, x * stepSize);
				float sample = Noise.Sum(point, frequency, octaves, lacunarity, persistence) + 0.5f;
				colors[v] = coloring.Evaluate(sample);
				sample *= strength / frequency;
				vertices[v].y = sample / zoomScale;
			}
		}
		terrainMesh.vertices = vertices;
		terrainMesh.colors = colors;
		terrainMesh.RecalculateNormals ();
	}

	// Creates a flat grid of vertices and triangles with the set resolution
	public void CreateGrid() {

		// Refresh the mesh
		terrainMesh.Clear ();

		// Create the vertices
		vertices = new Vector3[(resolution + 1) * (resolution + 1)];
		colors = new Color[vertices.Length];
		float stepSize = 1f / resolution;
		for (int v = 0, z = 0; z <= resolution; z++) {
			for (int x = 0; x <= resolution; x++, v++) {
				vertices[v] = new Vector3(x * stepSize - 0.5f, 0f, z * stepSize - 0.5f);
			}
		}
		terrainMesh.vertices = vertices;
		terrainMesh.colors = colors;

		// Determine the triangles
		int[] triangles = new int[resolution * resolution * 6];
		for (int t = 0, v = 0, y = 0; y < resolution; y++, v++) {
			for (int x = 0; x < resolution; x++, v++, t += 6) {
				triangles[t] = v;
				triangles[t + 1] = v + resolution + 1;
				triangles[t + 2] = v + 1;
				triangles[t + 3] = v + 1;
				triangles[t + 4] = v + resolution + 1;
				triangles[t + 5] = v + resolution + 2;
			}
		}
		terrainMesh.triangles = triangles;
	}
}
