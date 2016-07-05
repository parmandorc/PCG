using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Singleton class for the control of the upper layers generation, that is, water bodies and roads.
public class UpperLayersManager : MonoBehaviour {

	// Static singleton property
	public static UpperLayersManager Instance { get ; private set; }

	// Reference to the UI editor.
	public UpperLayersEditor ULE;

	// Container for the definition data for all water bodies.
	private List<Vector3> waterBodiesDefinitionData;

	// Container for the results of all water bodies.
	private List<List<Vector3>> waterBodies;

	// Container for the currently running water bodies generation threads.
	// Note: It's not a list because thread n+1 might finish before thread n.
	private Dictionary<int, WaterBodyGenerator> waterBodiesThreads;

	// Reference to the currently running water bodies handling coroutine.
	private Coroutine waterBodiesCoroutine = null;

	// Result of the postprocess of water bodies.
	private Dictionary<Vector2, float> waterBodiesProcessedLayer;

	// Reference to the currently running postprocess of water bodies thread.
	private WaterBodiesLayerPostprocess waterBodiesPostprocessThread = null;

	// Reference to the currently running postprocess of water bodies handling coroutine.
	private Coroutine waterBodiesPostprocessCoroutine = null;

	// Container for the definition data for all roads.
	private List<Eppy.Tuple<Vector2, Vector2, float, float>> roadsDefinitionData;

	// Container for the results of all roads.
	private List<List<Vector3>> roads;

	// Container for the currently running road generation threads.
	// Note: same as waterBodiesThreads.
	private Dictionary<int, RoadGenerator> roadThreads;

	// Reference to the currently running road generation handling coroutine.
	private Coroutine roadsCoroutine = null;

	void Awake () {
		// First we check if there are any other instances conflicting
		if(Instance != null && Instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		// Here we save our singleton instance
		Instance = this;

		waterBodiesDefinitionData = new List<Vector3> ();
		waterBodies = new List<List<Vector3>> ();
		waterBodiesProcessedLayer = new Dictionary<Vector2, float> ();
		waterBodiesThreads = new Dictionary<int, WaterBodyGenerator> ();

		roadsDefinitionData = new List<Eppy.Tuple<Vector2, Vector2, float, float>> ();
		roads = new List<List<Vector3>> ();
		roadThreads = new Dictionary<int, RoadGenerator> ();
	}

	public List<List<Vector3>> getWaterBodies() {
		return new List<List<Vector3>>(waterBodies);
	}

	public Dictionary<Vector2, float> getWaterBodiesProcessedLayer() {
		return this.waterBodiesProcessedLayer;
	}

	public List<List<Vector3>> getRoads() {
		return new List<List<Vector3>> (roads);
	}

	// Coroutine that handles the syncronization for all currently running water bodies generation threads.
	private IEnumerator WaitForWaterBodyThreads() {

		while (waterBodiesThreads.Count > 0) {

			// Cannot delete threads from container while iterating over it.
			List<int> toRemove = new List<int> ();

			foreach (int i in waterBodiesThreads.Keys) {
				
				//Wait until the thread is done
				if (waterBodiesThreads [i].IsDone) {
					
					List<Vector3> result = waterBodiesThreads[i].output;

					waterBodies[i] = result;

					toRemove.Add (i);

					// Update UI status
					ULE.setWaterBodyStatus (i, (result.Count == 0) ? UIElement.UIStatus.Error : UIElement.UIStatus.OK);
				}
			}

			foreach (int i in toRemove)
				waterBodiesThreads.Remove (i);

			// If any results where obtain, apply them to the terrain chunks.
			if (toRemove.Count > 0)
				ChunkLoader.Instance.ReloadAllChunks_OnlyUpperLayers ();

			yield return null;
		}

		waterBodiesCoroutine = null;

		// When all currently running threads are finished, launch the postprocess of water bodies.
		if (waterBodiesPostprocessCoroutine != null)
			StopCoroutine (waterBodiesPostprocessCoroutine);
		if (waterBodiesPostprocessThread != null)
			waterBodiesPostprocessThread.Abort ();
		waterBodiesPostprocessThread = new WaterBodiesLayerPostprocess ();
		waterBodiesPostprocessThread.Start ();
		waterBodiesPostprocessCoroutine = StartCoroutine (WaitForWaterBodiesPostprocessThread ());
	}

	// Launches the generation of the specified water body (or all of them if index = -1).
	public void RecalculateWaterBodies(int index = -1) {

		// Reset all roads, since roads will have to be recalculated after any water body change.
		foreach (RoadGenerator thread in roadThreads.Values)
			thread.Abort ();
		roadThreads.Clear ();
		if (roadsCoroutine != null) {
			StopCoroutine (roadsCoroutine);
			roadsCoroutine = null;
		}
			
		for (int i = ((index < 0) ? 0 : index); i < ((index < 0) ? waterBodiesDefinitionData.Count : (index + 1)); i++) {

			if (waterBodiesThreads.ContainsKey (i)) // If thread for this water body is running, substitute it.
				waterBodiesThreads[i].Abort ();

			Vector3 origin = waterBodiesDefinitionData [i];
			WaterBodyGenerator thread;
			if (origin.y >= 0f)
				thread = new OceanGenerator (new Vector2 (origin.x, origin.z), origin.y);
			else
				thread = new RiverGenerator (new Vector2 (origin.x, origin.z));
			thread.Start ();

			waterBodiesThreads[i] = thread;
		}

		// If the coroutine is already launch, no change is needed, since it will check for all currently running threads.
		if (waterBodiesCoroutine == null)
			waterBodiesCoroutine = StartCoroutine (WaitForWaterBodyThreads());
	}

	// Coroutine that handles syncronization of the postprocess of water bodies thread.
	private IEnumerator WaitForWaterBodiesPostprocessThread()
	{
		while (!waterBodiesPostprocessThread.IsDone)
			yield return null;
		this.waterBodiesProcessedLayer = waterBodiesPostprocessThread.result;
		waterBodiesPostprocessThread = null;

		// Since this is only called when a change in water bodies is made, roads have to be recalculated.
		RecalculateRoads ();
	}

	// Coroutine that handles syncronization of the currently running road generation threads.
	private IEnumerator WaitForRoadThreads() {

		while (roadThreads.Count > 0) {

			// Cannot delete threads from container while iterating over it.
			List<int> toRemove = new List<int> ();

			foreach (int i in roadThreads.Keys) {

				// Wait until the thread is done
				if (roadThreads [i].IsDone) {
					
					List<Vector3> result = roadThreads [i].output;

					roads [i] = result;

					toRemove.Add (i);

					// Update UI status
					ULE.setRoadStatus (i, (result.Count == 0) ? UIElement.UIStatus.Error : UIElement.UIStatus.OK);
				}
			}

			foreach (int i in toRemove)
				roadThreads.Remove (i);

			// If any results where obtain, apply them.
			if (toRemove.Count > 0)
				ChunkLoader.Instance.ReloadAllChunks_OnlyUpperLayers ();
			
			yield return null;
		}

		roadsCoroutine = null;
	}

	// Launches the generation of the specified road (or all of them if index = -1).
	public void RecalculateRoads(int index = -1) {

		for (int i = ((index < 0) ? 0 : index); i < ((index < 0) ? roadsDefinitionData.Count : (index + 1)); i++) {

			if (roadThreads.ContainsKey (i)) // If thread already running, substitute it.
				roadThreads [i].Abort ();

			Eppy.Tuple<Vector2, Vector2, float, float> roadData = roadsDefinitionData [i];
			RoadGenerator thread = new RoadGenerator (roadData.Item1, roadData.Item2, roadData.Item3, roadData.Item4);
			thread.Start ();

			roadThreads [i] = thread;
		}

		// If the coroutine is already launch, no change is needed, since it will check for all currently running threads.
		if (roadsCoroutine == null)
			roadsCoroutine = StartCoroutine(WaitForRoadThreads());
	}

	// Launches the generation of all water bodies and roads, and deletes all existing results.
	public void RecalculateUpperLayers() {

		for (int i = 0; i < waterBodies.Count; i++) {
			waterBodies[i].Clear ();
			ULE.setWaterBodyStatus (i, UIElement.UIStatus.Loading);
		}

		for (int i = 0; i < roads.Count; i++) {
			roads[i].Clear ();
			ULE.setRoadStatus (i, UIElement.UIStatus.Loading);
		}

		RecalculateWaterBodies ();
	}

	public void newWaterBody(Vector3 origin) {
		waterBodiesDefinitionData.Add (origin);
		waterBodies.Add (new List<Vector3> ());
	}

	public void changeWaterBody(int index, Vector3 newOrigin) {
		waterBodiesDefinitionData [index] = newOrigin;
		waterBodies [index].Clear ();
		ULE.setWaterBodyStatus (index, UIElement.UIStatus.Loading);
		for (int i = 0; i < roads.Count; i++) {
			roads[i].Clear ();
			ULE.setRoadStatus (i, UIElement.UIStatus.Loading);
		}
		ChunkLoader.Instance.ReloadAllChunks ();
		RecalculateWaterBodies (index);
	}

	public void newRoad(Vector2 origin, Vector2 destination, float maxSlope, float bridgePenalty) {
		roadsDefinitionData.Add (new Eppy.Tuple<Vector2, Vector2, float, float> (origin, destination, maxSlope, bridgePenalty));
		roads.Add (new List<Vector3> ());
	}

	public void changeRoad(int index, Vector2 newOrigin, Vector2 newDestination, float newMaxSlope, float newBridgePenalty) {
		roadsDefinitionData [index] = new Eppy.Tuple<Vector2, Vector2, float, float> (newOrigin, newDestination, newMaxSlope, newBridgePenalty);
		roads [index].Clear ();
		ULE.setRoadStatus (index, UIElement.UIStatus.Loading);
		ChunkLoader.Instance.ReloadAllChunks ();
		RecalculateRoads (index);
	}
}
