using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : Singleton
{
    public Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        pathFinding = this;
    }

    private void OnDestroy()
    {
        pathFinding = null;
    }

    public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openSet);
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                List<Vector2> smoothedPath = RetracePath(startNode, targetNode);
                return SmoothPath(smoothedPath);
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // Path not found
        return null;
    }

    private Node GetLowestFCostNode(List<Node> openSet)
    {
        Node lowestNode = openSet[0];
        for (int i = 1; i < openSet.Count; i++)
        {
            if (openSet[i].fCost < lowestNode.fCost || (openSet[i].fCost == lowestNode.fCost && openSet[i].hCost < lowestNode.hCost))
            {
                lowestNode = openSet[i];
            }
        }
        return lowestNode;
    }

    private List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        return dstX + dstY;
    }

    private List<Vector2> SmoothPath(List<Vector2> path)
    {
        List<Vector2> smoothedPath = new List<Vector2>();
        if (path.Count < 2)
            return path;

        Vector2 previousDirection = (path[1] - path[0]).normalized;
        smoothedPath.Add(path[0]);

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector2 currentDirection = (path[i + 1] - path[i]).normalized;
            if (currentDirection != previousDirection)
            {
                smoothedPath.Add(path[i]);
            }
            previousDirection = currentDirection;
        }

        smoothedPath.Add(path[path.Count - 1]);
        return smoothedPath;
    }
}

//using System.Collections.Generic;
//using UnityEngine;

//public class Pathfinding : MonoBehaviour
//{
//    public Grid grid;

//    private void Awake()
//    {
//        grid = GetComponent<Grid>();
//    }

//    public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
//    {
//        Node startNode = grid.NodeFromWorldPoint(startPos);
//        Node targetNode = grid.NodeFromWorldPoint(targetPos);

//        List<Node> openSet = new List<Node>();
//        HashSet<Node> closedSet = new HashSet<Node>();

//        openSet.Add(startNode);

//        while (openSet.Count > 0)
//        {
//            Node currentNode = openSet[0];

//            for (int i = 1; i < openSet.Count; i++)
//            {
//                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
//                {
//                    currentNode = openSet[i];
//                }
//            }

//            openSet.Remove(currentNode);
//            closedSet.Add(currentNode);

//            if (currentNode == targetNode)
//            {
//                return RetracePath(startNode, targetNode);
//            }

//            foreach (Node neighbor in grid.GetNeighbors(currentNode))
//            {
//                if (!neighbor.walkable || closedSet.Contains(neighbor))
//                {
//                    continue;
//                }

//                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

//                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
//                {
//                    neighbor.gCost = newCostToNeighbor;
//                    neighbor.hCost = GetDistance(neighbor, targetNode);
//                    neighbor.parent = currentNode;

//                    if (!openSet.Contains(neighbor))
//                    {
//                        openSet.Add(neighbor);
//                    }
//                }
//            }
//        }

//        // Path not found
//        return null;
//    }

//    private List<Vector2> RetracePath(Node startNode, Node endNode)
//    {
//        List<Vector2> path = new List<Vector2>();
//        Node currentNode = endNode;

//        while (currentNode != startNode)
//        {
//            path.Add(currentNode.worldPosition);
//            currentNode = currentNode.parent;
//        }

//        path.Reverse();

//        return path;
//    }

//    private int GetDistance(Node nodeA, Node nodeB)
//    {
//        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
//        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

//        return dstX + dstY;
//    }
//}