using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName="MySubMenue/Create GridSetting ")]
public class GridSetting : ScriptableObject
{
    public MyGrid<PathNode> grid;
    public Vector3 leftDown = new Vector3(-26.92f, -17.96f, 0);
    public Vector3 rightUP = new Vector3(25.32f, 7.37f, 0);
    public  GridSetting( )
    {
        grid = new MyGrid<PathNode>(leftDown, rightUP, (position,x,y) => { return new PathNode(position, x,y,true); });
    }
}
