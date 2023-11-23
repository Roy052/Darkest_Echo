using System.Text;
using UnityEngine;
using System.Collections.Generic;

public class Grid : Singleton
{
    public LayerMask unwalkableMask;
    public Vector2 gridSize;
    public float nodeRadius;
    public float objectRadius;
    public Node[,] grid;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        gridInstance = this;
    }

    private void OnDestroy()
    {
        gridInstance = null;
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridSize.x / 2 - Vector2.up * gridSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, objectRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
                if(walkable == false)
                {
                    //Debug.Log($"Pos : {x}, {y}");
                }
            }
        }

        //OnLogGrid();
    }

    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x + gridSize.x / 2) / gridSize.x);
        float percentY = Mathf.Clamp01((worldPosition.y + gridSize.y / 2) / gridSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbors.Add(grid[checkX, checkY]);
            }
        }

        return neighbors;
    }

    public Vector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path.ToArray();
    }

    private void OnLogGrid()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = gridSizeX - 1; i >= 0; i--)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                sb.Append($"{grid[i, j].walkable} ");
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }
}
