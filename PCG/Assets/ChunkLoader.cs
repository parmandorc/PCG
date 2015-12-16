using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkLoader : MonoBehaviour 
{
	public TerrainChunk terrainChunkPrefab;
	
	private Vector2? lastChunkID = null;

	// Table of all the chunks loaded in the scene (key = Vector2("chunkID"), value = TerrainChunk)
	private Dictionary<Vector2, TerrainChunk> chunks;

	// Used to know what chunks have to be updated. Whenever a change in terrain characteristics occur, version is incremented.
	private long currentVersion = 0;

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

		chunks = new Dictionary<Vector2, TerrainChunk>();
	}

	private void Update () {

		// Calculate the chunk the player is in
		Vector3 pos = gameObject.transform.position;
		Vector2 currentChunkID = new Vector2((int)(pos.x >= 0 ? pos.x : pos.x - 1),(int)(pos.z >= 0 ? pos.z : pos.z - 1));

		if (currentChunkID != lastChunkID) { //Update only if player changed chunks
			List<Vector2> chunkIDs = getChunkIDsToLoad(currentChunkID);

			foreach (Vector2 chunkID in chunkIDs) {
				// Create unvisited chunks
				if (!chunks.ContainsKey(chunkID))
				{
					TerrainChunk chunk = Instantiate(terrainChunkPrefab).Init(chunkID, currentVersion);
					chunks.Add(chunkID, chunk);
				}
				else {
					// Reload outdated chunks
					TerrainChunk chunk = chunks[chunkID];
					if (chunk.version < currentVersion) {
						chunk.CalculateValues();
						chunk.version = currentVersion;
					}
				}
			}
			
			lastChunkID = currentChunkID;
		}
	}

	public void ReloadAllChunks() {
		// Whenever a change in terrain characteristics occur, the version is incremented.
		currentVersion++;

		List<Vector2> chunkIDs = getChunkIDsToLoad(lastChunkID.Value);

		// Reload all chunks. Full reload for loaded chunks, version invalidation for others.
		foreach (Vector2 chunkID in chunks.Keys) {
			if (chunkIDs.Contains(chunkID))
			{
				TerrainChunk chunk = chunks[chunkID];
				chunk.CalculateValues();
				chunk.version = currentVersion;
			}
		}
	}

	// Returns the IDs of the chunks that should be loaded. Note: not only the one the player is in, but also the surrounding ones so that navigation is fluid.
	private List<Vector2> getChunkIDsToLoad(Vector2 chunkID)
	{
		int mapSize = TerrainCharacteristicsManager.Instance.mapSize - 1;
		List<Vector2> IDs = new List<Vector2> ();
		for (float x = Mathf.Max(chunkID.x - 1, 0f); x <= Mathf.Min(chunkID.x + 1, mapSize); x++) {
			for (float y = Mathf.Max(chunkID.y, 0f); y <= Mathf.Min(chunkID.y + 2, mapSize); y++) {
				IDs.Add(new Vector2(x, y));
			}
		}
		return IDs;
	}
}
