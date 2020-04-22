using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using Some.Namespace.That.Gets.PriorityQueue;

public class AStar : MonoBehaviour {

    Dictionary<int, List<Node>> attachedToWaypoint = new Dictionary<int, List<Node>>();
    List<GameObject> finalPath = new List<GameObject>();

    Node startNode;
    Node targetNode;

    GameObject[] waypoints;
    GameObject startGameObject;
    GameObject targetGameObject;
    NavManager navManager;
    //GameObject current;

    //PriorityQueue<float, GameObject> open = new PriorityQueue<float, GameObject>();
    float NEIGHBOR_DISTANCE = 50f;  // was 50
    float distanceToUser;
    float distanceToPlayer;
    float nodeDistances;

    GameObject user;
    //public GameObject player;
    //public GameObject parent;

    // Use this for initialization
    void Start() {
        user = gameObject.GetComponent<NavManager>()._camera;
        navManager = gameObject.GetComponent<NavManager>();
    }

    public List<GameObject> generateRTHPath(GameObject home) {
        //Debug.Log("Generating RTH path...");
        distanceToUser = 1000;
        //distanceToPlayer = 1000;
        //nodeDistances = 10000;

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        if (waypoints.Length == 0) {
            DebugManager.Instance.LogBoth("ASTAR", "ERROR: there are no active waypoints");
            return null;
        }

        targetGameObject = home;

        storeNodesInDictionary();

        findPath(startGameObject.transform.position, targetGameObject.transform.position);

        return finalPath;
    }

    void storeNodesInDictionary() {
        //Debug.Log("Num of starting waypoints: " + waypoints.Length);

        for (int i = 0; i < waypoints.Length; i++) {
            var waypoint = waypoints[i];
            var nodeData = new List<Node>();
            var waypointPosition = waypoint.transform.position;

            float distUser = (user.transform.position - waypointPosition).sqrMagnitude; // Closest node to user
            if (distUser < distanceToUser * distanceToUser) {
                //Debug.Log("Updating start pos");
                startGameObject = waypoint;
                distanceToUser = distUser;
            }

            //for (int j = 0; j < waypoints.Length; j++) {
            //    if (i == j)
            //        continue;
                
            //    var otherWaypoint = waypoints[j];
            //    float distanceSqr = (waypointPosition - otherWaypoint.transform.position).sqrMagnitude; // Gets distance values
            //    if (distanceSqr < 60) // Is waypoints are within a spcific distance
            //    {

            //        nodeData.Add(new Node(otherWaypoint.GetInstanceID(), otherWaypoint.transform.position));
            //    }
            //}
            //attachedToWaypoint.Add(waypoint.GetInstanceID(), nodeData); // Adds parent node and neighbouring nodes within a 3x3 grid
        }
    }

    List<Node> GetNeighbours(Node seachNode, List<Node> nodes) {
        List<Node> neighbors = new List<Node>();
        neighbors.Clear();

        foreach (Node node in nodes) {
            if (node.worldPosition == seachNode.worldPosition)
                continue;

            
            if ((node.worldPosition - seachNode.worldPosition).sqrMagnitude < NEIGHBOR_DISTANCE * NEIGHBOR_DISTANCE) {
                neighbors.Add(node);
            }
        }

       // Debug.Log("Num of neighbors: " + neighbors.Count);

        if (neighbors.Count == 0) {
            DebugManager.Instance.LogBoth("RTH ERROR", "No neighbors found, ASTAR failed. This means the user walked faster than " + NEIGHBOR_DISTANCE + "m/s. Adjust NEIGHBOR_DISTANCE");
            ErrorHandler.Instance.HandleError(1, "RTH ERROR: Unable to find path. See console.");
            navManager.PressRTH();
        }

        //Debug.Log("Node " + seachNode.worldPosition + " has this many neighbors: " + neighbors.Count);

        //foreach (Node tmp in neighbors) {
        //    Debug.Log("POS: " + tmp.worldPosition);
        //}
        
        return neighbors;
    }

    void findPath(Vector3 startPos, Vector3 targetPos) {
        startNode = new Node(startPos);
        targetNode = new Node(targetPos);
       
        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        List<Node> temp = new List<Node>();
        List<Node> nodes = new List<Node>();

        //Debug.Log("TEST: " + startNode.fCost);
        //Debug.Log("TEST2: " + startNode.hCost);
        //Debug.Log("TEST3: " + startNode.gCost);
        Debug.Log("------ ASTAR ------");
        Debug.Log("Start position   : " + startNode.worldPosition);
        Debug.Log("Target position  : " + targetNode.worldPosition);

        foreach (GameObject obj in waypoints) {
            // add every node EXEPT the start and end nodes
            if (obj.transform.position != startNode.worldPosition && obj.transform.position != targetNode.worldPosition) {
                temp.Add(new Node(obj.GetInstanceID(), obj.transform.position));
            }
        }

        // remove duplicates
        nodes = temp.Distinct().ToList();
        int startPosIndex = 1;
        foreach (Node node in nodes) {
            for (int i=startPosIndex; i<nodes.Count; i++) {
                if (node.worldPosition == nodes[i].worldPosition) {
                    nodes[i].gCost = -1;
                }
            }
            startPosIndex++;
        }

        nodes.RemoveAll(item => item.gCost == -1);

        // add target node
        nodes.Add(targetNode);

        //Debug.Log("Unique Nodes: " + nodes.Count);
        

        // Start ASTAR
        openSet.Add(startNode);

       // Debug.Log("Target node: " + targetNode.localNodeID);

        while (openSet.Count > 0) {
            Node currentNode = openSet[0];

            for (int i=1; i<openSet.Count; i++) {
                //Debug.Log("1: " + openSet[i].fCost + " - " + currentNode.fCost);

                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost) {

                    //Debug.Log("2: " + openSet[i].hCost + " - " + currentNode.hCost);
					if (openSet[i].hCost < currentNode.hCost)
						currentNode = openSet[i];
				}
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            //Debug.Log("Current node: " + currentNode.localNodeID + " " + currentNode.worldPosition);

            if (currentNode == targetNode) {
                //Debug.Log("Goal reached");
                RetracePath();
                return;
            }
            
            foreach (Node neighbor in GetNeighbours(currentNode, nodes)) {
                //Debug.Log("Checking neighbor: " + neighbor.localNodeID + " " + neighbor.worldPosition);

                if (closedSet.Contains(neighbor)) {
                    continue;
                }

                //int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbor);
                int newMovementCostToNeighbour = currentNode.gCost + (int)Vector3.Distance(currentNode.worldPosition, neighbor.worldPosition);
                if (newMovementCostToNeighbour < neighbor.gCost || !openSet.Contains(neighbor)) {
                    neighbor.gCost = newMovementCostToNeighbour;
                    neighbor.hCost = (int)Vector3.Distance(neighbor.worldPosition, targetNode.worldPosition);
                    neighbor.parent = currentNode;
                    //Debug.Log("Setting " + neighbor.localNodeID + " (" + neighbor.worldPosition + ") parent as " + neighbor.parent.localNodeID + " (" + currentNode.worldPosition + ")");

                    if (!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    int GetDistance(Node nodeA, Node nodeB) {
		int dstX = (int)Mathf.Abs(nodeA.worldPosition.x - nodeB.worldPosition.x);
		int dstY = (int)Mathf.Abs(nodeA.worldPosition.z - nodeB.worldPosition.z);
		
		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

    void RetracePath() {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

       // Debug.Log("Start: " + startNode.localNodeID);

        while (currentNode != startNode) {
           // Debug.Log("Current: " + currentNode.localNodeID);
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        List<GameObject> returnPath = new List<GameObject>();
        foreach (Node node in path) {
            GameObject tmp = new GameObject();
            tmp.transform.position = node.worldPosition;
            returnPath.Add(tmp);
        }

        finalPath = returnPath;
    }
}

public class Node {
    public Vector3 worldPosition;

    public int associatedInstanceID;
    public int localNodeID;
    public int gCost = 0;
    public int hCost = 0;
    public Node parent;

    static int nodesCounter = 0;

    public Node(int _instanceID, Vector3 _worldPos) {
        worldPosition = _worldPos;
        associatedInstanceID = _instanceID;
        localNodeID = nodesCounter;

        //Debug.Log("NEW ID: " + localNodeID);
        nodesCounter++;
    }

    public Node(Vector3 _worldPos) {
        worldPosition = _worldPos;
        localNodeID = nodesCounter;

        //Debug.Log("NEW ID: " + localNodeID);
        nodesCounter++;
    }

    public float fCost {
        get {
            return gCost + hCost;
        }
    }
}
