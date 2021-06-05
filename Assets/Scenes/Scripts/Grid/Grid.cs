using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    
    public Grid(int _width, int _height, float _cellSize)
    {
        this.width = _width;
        this.height = _height;
        this.cellSize = _cellSize;
        gridArray = new int[width, height];
        drawGrid();
    }

    private void drawGrid()
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(getWorldPosition(x, y), getWorldPosition(x + 1, y), Color.red, 100f);
                Debug.DrawLine(getWorldPosition(x, y), getWorldPosition(x , y+1), Color.red, 100f);
            }
        }
        Debug.DrawLine(getWorldPosition(0, height), getWorldPosition(width, height), Color.red, 100f);
        Debug.DrawLine(getWorldPosition(width, 0), getWorldPosition(width, height), Color.red, 100f);
    }

    private Vector3 getWorldPosition(int x, int y)
    {
        return new Vector3(x,y)*cellSize;
    }
    
}
