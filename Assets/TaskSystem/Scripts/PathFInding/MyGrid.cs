using System;
using CodeMonkey;
using UnityEngine;

public struct XY
{
    private int X;
    private int Y;

    public XY(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int GetX()
    {
        return X;
    }
    public int GetY()
    {
        return Y;
    }
}
public class MyGrid<TgridObect>
{
    private const float CellSize = 1f;
    private int width;
    private int height;
    private TgridObect[,] gridArray;
    private Vector3 originPosition;
    
    public MyGrid(int width,int height,Func<Vector3,int,int,TgridObect> createGridContent)
    {
        originPosition = new Vector3(0, 0, 0);
        this.width = width;
        this.height = height;
        Vector3 startPosition;
        Vector3 upPosition;
        Vector3 rightPosition;
        gridArray = new TgridObect[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                startPosition = originPosition + (Vector3.right) * i * CellSize + Vector3.up * j * CellSize;
                upPosition = startPosition + Vector3.up * CellSize;
                rightPosition = startPosition + Vector3.right * CellSize;
                Debug.DrawLine(startPosition, upPosition, Color.red, 100f);
                Debug.DrawLine(startPosition, rightPosition, Color.red, 100f);
                gridArray[i, j] = createGridContent(startPosition + (Vector3.right) * 0.5f * CellSize,i,j);
                // MyClass.CreateWorldText(null, "1", startPosition + (Vector3.right) * 0.5f * CellSize, 10, Color.red,
                    // TextAnchor.MiddleCenter, TextAlignment.Center, 1);
            }
        }
    }
    public TgridObect GetGridObject(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
            return gridArray[x, z];
        else
        {
            return default(TgridObect);;
        }
    }

    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }

    public XY GetXY(Vector3 pos)
    {
        int x = Mathf.FloorToInt((pos - originPosition).x / CellSize);
        int y = Mathf.FloorToInt(pos.y / CellSize);
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return new XY(x, y);
        }
        Debug.Log("区域错误！");
        return default(XY);
    }
}
