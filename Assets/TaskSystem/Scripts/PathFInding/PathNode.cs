using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public GameHandler.ItemManager itemManager;
   
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
        itemManager = null;
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

    public void setItemManager(GameHandler.ItemManager _itemManager)
    {
        itemManager = _itemManager;
    }

}
