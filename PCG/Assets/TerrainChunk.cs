using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainChunk : MonoBehaviour {

	// Used by the ChunkLoader to know what chunks have to be updated.
	public long version;

	// The mesh of the generated terrain
	private Mesh terrainMesh;

	private Vector2 chunkID;

	private int resolution = 100;

	private Vector3[] vertices;

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


	// Update is called once per frame
	void Update () {
	
	}

	public void CalculateValues() {
		bool substitutingThread = false;

		// If a value calculation thread is executing, we just substitute it
		if (thread != null) {
			thread.Abort ();
			substitutingThread = true;
		}

		// Launch the value calculation thread
		TerrainCharacteristicsManager tcm = TerrainCharacteristicsManager.Instance;
		thread = new ValueCalculationThreadedJob (chunkID, resolution, vertices, tcm.getTerrainAreasDeepCopy ());
		thread.Start();

		// If the thread was already launched, and thus the coroutine already existed, we do not neet to start the waiting coroutine again.
		// The ongoing coroutine will now check for the thread we just created.
		if (!substitutingThread)
			StartCoroutine (WaitForValueCalculationThread ());

	}

	private IEnumerator WaitForValueCalculationThread() {
		//Wait until the thread is done
		while (!thread.IsDone)
			yield return null;

		//Obtain the results and apply them
		terrainMesh.vertices = thread.vertices;
		terrainMesh.colors = thread.colors;

		thread = null;
	}
}
