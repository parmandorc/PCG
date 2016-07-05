using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class responsible for the representation and control of a terrain chunk in the level streaming system.
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainChunk : MonoBehaviour {

	// Used by the ChunkLoader for version-invalidation updates.
	public long version;

	// Used for flag-invalidation updates.
	public bool mustApplyUpperLayers = false;

	// The mesh of the generated terrain
	private Mesh terrainMesh;

	// The ID of the terrain chunk associated with this object.
	private Vector2 chunkID;

	// The resolution in vertices of the mesh
	public static int resolution = 128;

	// The vertices array of the mesh
	private Vector3[] vertices;

	// Reference to the currently working value calculation thread
	private ValueCalculationThreadedJob thread = null;

	// Use this for initialization
	void Awake () {
		terrainMesh = new Mesh ();
		terrainMesh.name = "Terrain Mesh";
		GetComponent<MeshFilter>().mesh = terrainMesh;
	}

	public TerrainChunk Init(Vector2 chunkID, long version) {
		this.version = version;

		gameObject.transform.position = new Vector3 (chunkID.x + 0.5f, 0f, chunkID.y + 0.5f);

		this.chunkID = chunkID;

		// Create grid	
		vertices = new Vector3[(resolution + 1) * (resolution + 1)];
		float stepSize = 1f / resolution;
		for (int v = 0, z = 0; z <= resolution; z++) {
			for (int x = 0; x <= resolution; x++, v++) {
				vertices[v] = new Vector3(x * stepSize - 0.5f, 0f, z * stepSize - 0.5f);
			}
		}
		terrainMesh.vertices = vertices;

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
		
		// CalculateValues
		CalculateValues ();

		return this;
	}

	public void CalculateValues() {
		bool substitutingThread = false;

		// If a value calculation thread is executing, we just substitute it
		if (thread != null) {
			thread.Abort ();
			substitutingThread = true;
		}

		// Launch the value calculation thread
		thread = new ValueCalculationThreadedJob (chunkID, vertices);
		thread.Start();

		// If the thread was already launched, and thus the coroutine already existed, we do not neet to start the waiting coroutine again.
		// The ongoing coroutine will now check for the thread we just created.
		if (!substitutingThread)
			StartCoroutine (WaitForValueCalculationThread ());

	}

	public void ApplyUpperLayers() {
		if (thread == null) {
			Vector3[] vertices = terrainMesh.vertices;
			Color[] colors = terrainMesh.colors;
			ApplyUpperLayer (UpperLayersManager.Instance.getWaterBodies(), Color.blue, vertices, colors);
			ApplyUpperLayer (UpperLayersManager.Instance.getRoads(), Color.black, vertices, colors);
			terrainMesh.vertices = vertices;
			terrainMesh.colors = colors;
		}
		mustApplyUpperLayers = false;
	}

	private IEnumerator WaitForValueCalculationThread() {
		//Wait until the thread is done
		while (!thread.IsDone)
			yield return null;

		//Obtain the results and apply them
		ApplyUpperLayer (UpperLayersManager.Instance.getWaterBodies(), Color.blue, thread.vertices, thread.colors);
		ApplyUpperLayer (UpperLayersManager.Instance.getRoads(), Color.black, thread.vertices, thread.colors);
		terrainMesh.vertices = thread.vertices;
		terrainMesh.colors = thread.colors;
		mustApplyUpperLayers = false;

		thread = null;
	}

	private void ApplyUpperLayer(List<List<Vector3>> layer, Color color, Vector3[] vertices, Color[] colors) {
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
