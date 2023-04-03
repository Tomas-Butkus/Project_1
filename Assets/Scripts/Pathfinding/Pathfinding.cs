using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target; // seeker and target tranform component
    public PathfindingGrid grid; // grid reference
    public PathfindingNode seekerNode, targetNode; // seeker and target node references
    public GameObject pathfindingGrid; // grid object in scene

    private void Start()
    {
        grid = pathfindingGrid.GetComponent<PathfindingGrid>(); // instantiate the grid
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //get player and target position in grid coords
        seekerNode = grid.NodeFromWorldPoint(startPos);
        targetNode = grid.NodeFromWorldPoint(targetPos);

        List<PathfindingNode> openSet = new List<PathfindingNode>();
        HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();
        openSet.Add(seekerNode);

        //calculates path for pathfinding
        while (openSet.Count > 0)
        {
            //iterates through openSet and finds lowest FCost
            PathfindingNode node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost <= node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            //If target found, retrace path
            if (node == targetNode)
            {
                RetracePath(seekerNode, targetNode);
                return;
            }

            //adds neighbor nodes to openSet
            foreach (PathfindingNode neighbour in grid.GetNeighbors(node))
            {
                if (neighbour.isObstacle || closedSet.Contains(neighbour))
                    continue;
                
                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parentNode = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    //reverses calculated path so first node is closest to seeker
    private void RetracePath(PathfindingNode startNode, PathfindingNode endNode)
    {
        List<PathfindingNode> path = new List<PathfindingNode>();
        PathfindingNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();

        grid.path = path;
    }

    //gets distance between 2 nodes for calculating cost
    int GetDistance(PathfindingNode nodeA, PathfindingNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
