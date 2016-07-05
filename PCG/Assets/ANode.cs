using UnityEngine;
using System.Collections;

// Class that represents a Node object used in the execution of the A* search algorithm
public class ANode {

	// The point associated with the node
	public Vector3 point { get; private set; }

	// Boolean that determines wether the point associated with the node belongs to a water body or not
	public bool inWaterBody { get; private set; }

	// Returns the coords in 2D of the point associated with the node
	public Vector2 point2D { get { return new Vector2 (point.x, point.z); } }

	// Pointer to the parent node in the A* search graph
	public ANode preceding;

	// G function of the node, which estimates the cost of the taken path
	public float G { get { return preceding == null ? 0f : (preceding.G + Vector3.Distance(point, preceding.point)) * ((inWaterBody || preceding.inWaterBody) ? bridgePenalty : 1f); } }

	// F function of the node, which estimates the cost of the taken path plus the cost of going from this node to the destination node.
	public float F { get { return G + Vector3.Distance (point, destination); } }

	// The point of destination in the search
	private Vector3 destination;

	// The penalty value to apply to the cost functions if the path between this node and its parent is a bridge
	private float bridgePenalty;

	// Base constructor
	public ANode(Vector3 point, bool inWaterBody, Vector3 destination, ANode preceding, float bridgePenalty) {
		this.point = point;
		this.inWaterBody = inWaterBody;
		this.destination = destination;
		this.preceding = preceding;
		this.bridgePenalty = bridgePenalty;
	}
}