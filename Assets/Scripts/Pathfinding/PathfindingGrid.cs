using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingGrid : MonoBehaviour
{
    [Header("Node Map Information:")]
    public int height = 10; // height of node map 
    public int width = 10; // width of node map 
    public float nodeDiameter = 1f; // spaces between nodes
    public float nodeRadius; // radius of the node

    public Vector3 gridWorldSize; 
    public PathfindingNode[,] grid; // list of nodes in the grid
    public Tilemap obstacleTileMap; // tilemap reference
    public List<PathfindingNode> path;
    Vector3 worldBottomLeft;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        width = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        height = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        GenerateGrid();
    }

    // Generate a map of all of the nodes
    private void GenerateGrid()
    {
        grid = new PathfindingNode[width, height];
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                grid[x, y] = new PathfindingNode(false, worldPoint, x, y);

                if (obstacleTileMap.HasTile(obstacleTileMap.WorldToCell(grid[x, y].worldPosition)))
                    grid[x, y].SetIsObstacle(true);
                else
                    grid[x, y].SetIsObstacle(false);
            }
        }
    }

    public List<PathfindingNode> GetNeighbors(PathfindingNode pathfindingNode)
    {
        List<PathfindingNode> neighbors = new List<PathfindingNode>();

        if (pathfindingNode.gridX >= 0 && pathfindingNode.gridX < width && pathfindingNode.gridY + 1 >= 0 && pathfindingNode.gridY + 1 < height) 
            neighbors.Add(grid[pathfindingNode.gridX, pathfindingNode.gridY + 1]);

        if (pathfindingNode.gridX >= 0 && pathfindingNode.gridX < width && pathfindingNode.gridY - 1 >= 0 && pathfindingNode.gridY - 1 < height) 
            neighbors.Add(grid[pathfindingNode.gridX, pathfindingNode.gridY - 1]);

        if (pathfindingNode.gridX + 1 >= 0 && pathfindingNode.gridX + 1 < width && pathfindingNode.gridY >= 0 && pathfindingNode.gridY < height) 
            neighbors.Add(grid[pathfindingNode.gridX + 1, pathfindingNode.gridY]);

        if (pathfindingNode.gridX - 1 >= 0 && pathfindingNode.gridX - 1 < width && pathfindingNode.gridY >= 0 && pathfindingNode.gridY < height)
            neighbors.Add(grid[pathfindingNode.gridX - 1, pathfindingNode.gridY]);
        
        return neighbors;
    }

    public PathfindingNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x - 0.5f + (width / 2));
        int y = Mathf.RoundToInt(worldPosition.y - 0.5f + (height / 2));

        Debug.Log("x axis: " + x + " y axis: " + y);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null)
        {
            foreach (PathfindingNode n in grid)
            {
                if (n.isObstacle)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.white;

                if (path != null && path.Contains(n))
                    Gizmos.color = Color.green;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeRadius));
            }
        }
    }
}
