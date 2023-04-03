using UnityEngine;

public class PathfindingNode
{
    [Header("Node Information:")]
    public int gridX, gridY; // grid coordinates
    public Vector3 worldPosition; // actual position in the scene
    public bool isObstacle; // is node walkable?
    [HideInInspector] public PathfindingNode parentNode; // previous node that helps to trace the shortest path

    [Header("Node costs:")]
    public int gCost; // the cost of moving to the next node
    public int hCost; // the distance to the goal from this node
    public int fCost { get { return gCost + hCost; } } // f cost = g cost - h cost


    public PathfindingNode(bool obstacle, Vector3 worldPos, int xCoordinate, int yCoordinate)
    {
        isObstacle = obstacle;
        worldPosition = worldPos;
        gridX = xCoordinate;
        gridY = yCoordinate;
    }

    public void SetIsObstacle(bool obstacle)
    {
        isObstacle = obstacle;
    }
}
