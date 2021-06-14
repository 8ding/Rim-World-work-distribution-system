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
    [SerializeField]
    public LayerMask layerMask;

    private void OnEnable()
    {
        grid = new MyGrid<PathNode>(leftDown, rightUP, (position,x,y,walkable) => {
            return new PathNode(position, x, y, walkable); },layerMask);
    }
}
