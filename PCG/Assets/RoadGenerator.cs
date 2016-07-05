using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class responsible for the A* search algorithm used in roads generation
public class RoadGenerator : ThreadedJob {

	// Output
	public List<Vector3> output;

	// Inputs
	private Vector3 destination;
	private float maxSlope;
	private float bridgePenalty;

	// Other needed
	private int resolution;
	private int mapSize;
	private Dictionary<Color, TerrainArea> terrainAreas;
	private Color[][] map;
	private TerrainArea optimizedTerrainArea;
	private Dictionary<Vector2, float> waterBodies;

	// Structures used by algorithm
	private SortedList<float, List<Vector2>> openList; //Used for constant time access of the lowest element
	private Dictionary<Vector2, ANode> openNodes;
	private Dictionary<Vector2, ANode> closedNodes;

	// Base constructor
	public RoadGenerator(Vector2 origin, Vector2 destination, float maxSlope, float bridgePenalty) {

		// Obtain all extra needed information
		this.resolution = TerrainChunk.resolution;
		this.mapSize = TerrainCharacteristicsManager.Instance.mapSize;
		this.terrainAreas = TerrainCharacteristicsManager.Instance.getTerrainAreasDeepCopy();
		object map = TerrainCharacteristicsManager.Instance.getChunkMap (new Vector2 (-1, -1));
		if (map is Color[][])
			this.map = (Color[][])map;
		else if (map is Color) // Optimized if only one color
			this.optimizedTerrainArea = terrainAreas[(Color)map];
		this.waterBodies = UpperLayersManager.Instance.getWaterBodiesProcessedLayer ();

		this.maxSlope = maxSlope;
		this.bridgePenalty = bridgePenalty;
		origin = new Vector2 (Mathf.Round (origin.x * resolution), Mathf.Round (origin.y * resolution));
		destination = new Vector2 (Mathf.Round (destination.x * resolution), Mathf.Round (destination.y * resolution));
		openList = new SortedList<float, List<Vector2>> ();
		openNodes = new Dictionary<Vector2, ANode> ();
		float originY = getHeightValue (origin);
		Vector3 origin3 = new Vector3 (origin.x, Mathf.Abs(originY), origin.y);
		this.destination = new Vector3 (destination.x, Mathf.Abs(getHeightValue (destination)), destination.y);
		closedNodes = new Dictionary<Vector2, ANode> ();

		if (isInsideMapBoundaries(origin)) {
			ANode originNode = new ANode (origin3, originY < 0, this.destination, null, bridgePenalty);
			openList [Vector3.Distance (origin3, destination)] = new List<Vector2> () { origin };
			openNodes [origin] = originNode;
		}
	}

	protected override void OnRun() {

		Vector2 destination2D = new Vector2 (destination.x, destination.z);
		ANode endNode = null;
		if (isInsideMapBoundaries (destination2D)) {
			while (openList.Count != 0) {
				// Pop open list
				Vector2 current = openList.Values [0] [0];
				ANode currentNode = openNodes [current];
				openList.Values [0].RemoveAt (0);
				if (openList.Values [0].Count == 0)
					openList.RemoveAt (0);
				openNodes.Remove (current);
				closedNodes [current] = currentNode;

				if (current != destination2D) {

					Vector2[] adjacents = {
						new Vector2 (current.x, current.y + 1),
						new Vector2 (current.x + 1, current.y),
						new Vector2 (current.x, current.y - 1),
						new Vector2 (current.x - 1, current.y)
					};

					foreach (Vector2 adj in adjacents) {
						if ((currentNode.preceding == null || adj != currentNode.preceding.point2D) && isInsideMapBoundaries(adj)) {
							if (openNodes.ContainsKey (adj) || closedNodes.ContainsKey (adj)) { // Update the node
								ANode adjNode = openNodes.ContainsKey (adj) ? openNodes [adj] : closedNodes [adj];
								if (Mathf.Abs (adjNode.point.y - currentNode.point.y) <= maxSlope) { 
									if (currentNode.G + Vector3.Distance (currentNode.point, adjNode.point) * ((currentNode.inWaterBody || adjNode.inWaterBody) ? bridgePenalty : 1f) < adjNode.G) {
										adjNode.preceding = currentNode;
									}
								}
							} else { // Create the node
								float adjY = getHeightValue (adj);
								Vector3 adj3D = new Vector3 (adj.x, Mathf.Abs (adjY), adj.y);
								if (Mathf.Abs (adj3D.y - currentNode.point.y) <= maxSlope) {
									ANode newNode = new ANode (adj3D, adjY < 0, destination, currentNode, bridgePenalty);
									float newF = newNode.F;
									if (!openList.ContainsKey (newF))
										openList [newF] = new List<Vector2> ();
									openList [newF].Add (newNode.point2D);
									openNodes [newNode.point2D] = newNode;
								}
							}
						}
					}

				} else {
					endNode = currentNode;
					break;
				}
			}
		}

		output = new List<Vector3> ();
		while (endNode != null) {
			output.Add (endNode.point / resolution);
			endNode = endNode.preceding;
		}
	}

	public float getHeightValue(Vector2 coords) {
		coords /= resolution;

		float value;
		if (waterBodies.ContainsKey(coords)) {
			value = - waterBodies[coords];
		} else {
			TerrainArea parameters = this.optimizedTerrainArea;
			if (parameters == null)
				parameters = Noise.getParameters (coords.x / mapSize, coords.y / mapSize, map, terrainAreas);

			value = Noise.FractalPerlin3D (new Vector3 (coords.x - 0.5f, coords.y - 0.5f, 0f), 
				parameters.frequency, 6, parameters.lacunarity, parameters.roughness, parameters.flatness, parameters.averageHeight);
		}

		return value * resolution;
	}

	private bool isInsideMapBoundaries(Vector2 point) {
		return point.x >= 0 && point.x <= mapSize * resolution && point.y >= 0 && point.y <= mapSize * resolution;
	}
}
