using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public GameHandler.ItemManager itemManager;
    public GameHandler.ResourceManager resourceManager;
    public bool isWalkable;
    public int gCost;
    public int hCost;
    public int fCost;
    public Vector3 worldPosition;
    public PathNode CamePathNode;
    public int x;
    public int y;
    public PathNode(Vector3 worldPosition,int x, int y,bool isWalkable)
    {
        itemManager = new GameHandler.ItemManager();
        resourceManager = new GameHandler.ResourceManager();
        this.worldPosition = worldPosition;
        this.isWalkable = isWalkable;
        this.x = x;
        this.y = y;
    }
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public void setWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
    
}
