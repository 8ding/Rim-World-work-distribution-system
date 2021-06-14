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
namespace TaskSystem.PathFInding
{
    public class MyGrid<TgridObect>
    {
        private const float CellSize = 1f;
        private int width;
        private int height;
        private TgridObect[,] gridArray;
        private Vector3 originPosition;
        
        public MyGrid(int width,int height,Func<Vector3,TgridObect> createGridContent)
        {
            originPosition = new Vector3(0, 0, 0);
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
                    gridArray[i, j] = createGridContent(startPosition + (Vector3.right) * 0.5f * CellSize);
                    MyClass.CreateWorldText(null, "1", startPosition + (Vector3.right) * 0.5f * CellSize, 10, Color.red,
                        TextAnchor.MiddleCenter, TextAlignment.Center, 1);
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
            return new XY(Mathf.FloorToInt((pos - originPosition).x / CellSize),
                Mathf.FloorToInt((pos - originPosition).y / CellSize));
        }
    }
}