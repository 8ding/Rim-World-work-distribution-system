using System.Collections;
using System.Collections.Generic;
using TaskSystem.PathFInding;
using UnityEngine;


public class PathFinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private MyGrid<PathNode> grid;
    private List<PathNode> opentList;
    private List<PathNode> closeList;
    
    public PathFinding(MyGrid<PathNode> grid)
    {
        this.grid = grid;
    }
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);
        
        opentList = new List<PathNode> {startNode};
        closeList = new List<PathNode> {};
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.CamePathNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (opentList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(opentList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            opentList.Remove(currentNode);
            closeList.Add(currentNode);
            foreach (var neighbourNode in GetNeighbourList(currentNode))
            {
                if(closeList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable)
                {
                    closeList.Add(neighbourNode);
                    continue;
                }
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.CamePathNode = currentNode;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();
                    if (!opentList.Contains(neighbourNode))
                    {
                        opentList.Add(neighbourNode);
                    }
                }
                
            }
        }

        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        if (currentNode.x - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            if (currentNode.y - 1 >= 0)
            {
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            }

            if (currentNode.y + 1 < grid.GetHeight())
            {
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            }
        }

        if (currentNode.x + 1 < grid.GetWidth())
        {
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            if (currentNode.y - 1 >= 0)
            {
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            }

            if (currentNode.y + 1 < grid.GetHeight())
            {
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            }
        }

        if (currentNode.y - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        }

        if (currentNode.y + 1 < grid.GetHeight())
        {
            neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));
        }

        return neighbourList;
    }
    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public PathNode GetNode(Vector3 pos)
    {
        XY xy = grid.GetXY(pos);
        return GetNode(xy.GetX(), xy.GetY());
    }
    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }
    private PathNode GetLowestFCostNode(List<PathNode> pathNodes)
    {
        PathNode lowestFCostNode = pathNodes[0];
        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (pathNodes[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodes[i];
            }
        }

        return lowestFCostNode;
    }
    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.CamePathNode != null)
        {
            path.Add(currentNode.CamePathNode);
            currentNode = currentNode.CamePathNode;
        }
        path.Reverse();
        return path;
    }

    public void DrawPath(List<PathNode> pathNodes)
    {
        for (int i = 0; i < pathNodes.Count; i++)
        {
            
        }
    }
}

