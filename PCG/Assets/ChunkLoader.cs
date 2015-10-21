using UnityEngine;
using System.Collections;

public class ChunkLoader : MonoBehaviour 
{
	public TerrainChunk terrainChunkPrefab;

	public Gradient coloring;

	public float frequency = 8f;

	public float lacunarity = 2f;

	public float persistence = 0.5f;

	public float strength = 1f;

	public TerrainCharacteristicsEditor TCE;

	private Vector2? lastChunkID = null;
	
	private Hashtable chunks;

	// Static singleton property
	public static ChunkLoader Instance { get ; private set; }
	
	void Awake () {
		// First we check if there are any other instances conflicting
		if(Instance != null && Instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}
		
		// Here we save our singleton instance
		Instance = this;


		chunks = new Hashtable();
	}

	void Start() {
		TCE.Init ();
	}

	public void UpdateTick (PlayerController player) {

		// Calculate the chunk the player is in
		Vector3 pos = player.transform.position;
		Vector2 chunkID = new Vector2((int)(pos.x >= 0 ? pos.x : pos.x - 1),(int)(pos.z >= 0 ? pos.z : pos.z - 1));

		//Debug.Log (chunkID);

		if (chunkID != lastChunkID) { //Update only if player changed chunks
			if (!chunks.Contains(chunkID))
			{
				TerrainChunk chunk = Instantiate(terrainChunkPrefab).Init(chunkID);
				chunks.Add(chunkID, chunk);
			}
		}

		lastChunkID = chunkID;
	}

	public void ReloadAllChunks() {
		foreach (TerrainChunk chunk in chunks.Values) {
			chunk.CalculateValues();
		}
	}
}
