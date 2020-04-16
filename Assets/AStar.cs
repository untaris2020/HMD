using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using Some.Namespace.That.Gets.PriorityQueue;

public class AStar : MonoBehaviour {

    Dictionary<GameObject, List<GameObject>> attachedToWaypoint = new Dictionary<GameObject, List<GameObject>>();

    GameObject[] waypoints;
    GameObject startGameObject;
    GameObject targetGameObject;
    //GameObject current;

    //PriorityQueue<float, GameObject> open = new PriorityQueue<float, GameObject>();

    float distanceToUser;
    float distanceToPlayer;
    float nodeDistances;

    public GameObject user;
    //public GameObject player;
    //public GameObject parent;

    // Use this for initialization
    void Start() {
        
    }

    public List<GameObject> generateRTHPath(GameObject home) {

        distanceToUser = 1000;
        //distanceToPlayer = 1000;
        //nodeDistances = 10000;

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        targetGameObject = new GameObject();
        targetGameObject = home;

        storeNodesInDictionary();

        findPath(startGameObject.transform.position, targetGameObject.transform.position);

        return null;
    }

    void storeNodesInDictionary() {
        for (int i = 0; i < waypoints.Length; i++) {
            var waypoint = waypoints[i];
            var nodeData = new List<GameObject>();
            var waypointPosition = waypoint.transform.position;

            float distUser = Vector3.Distance(user.transform.position, waypointPosition); // Closest node to user
            if (distUser < distanceToUser) {
                startGameObject = waypoint;
                distanceToUser = distUser;
            }

            for (int j = 0; j < waypoints.Length; j++) {
                if (i == j)
                    continue;
                
                var otherWaypoint = waypoints[j];
                float distanceSqr = (waypointPosition - otherWaypoint.transform.position).sqrMagnitude; // Gets distance values
                if (distanceSqr < 60) // Is waypoints are within a spcific distance
                {
                    nodeData.Add(otherWaypoint);
                }
            }
            attachedToWaypoint.Add(waypoint, nodeData); // Adds parent node and neighbouring nodes within a 3x3 grid
        }
    }

    void findPath(Vector3 startPos, Vector3 targetPos) {
        Node startNode = new Node(startPos);
        Node targetNode = new Node(targetPos);
       
        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        List<Node> temp = new List<Node>();
        List<Node> nodes = new List<Node>();

        foreach (GameObject obj in waypoints) {
            temp.Add(new Node(obj.transform.position));
        }
        nodes = temp.Distinct().ToList();
        openSet.Add(startNode);

        Debug.Log("------ ASTAR ------");
        Debug.Log("");
        Debug.Log("Start position   : " + startNode.worldPosition);
        Debug.Log("Target position  : " + targetNode.worldPosition);

        while (openSet.Count > 0) {
            Node currentNode = openSet[0];

            for (int i=1; i<openSet.Count; i++) {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                Debug.Log("Goal reached");
                break;
            }
            

        }
    }
}

public class Node {
    public Vector3 worldPosition;

    public int gCost;
    public int hCost;

    public Node(Vector3 _worldPos) {
        worldPosition = _worldPos;
    }

    public int fCost {
        get {
            return gCost + hCost;
        }
    }
}
