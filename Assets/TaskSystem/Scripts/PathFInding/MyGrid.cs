using UnityEngine;

namespace TaskSystem.PathFInding
{
    public class MyGrid
    {
        private const float CellSize = 1f;
        private int width;
        private int height;
        private int[,] gridArray;
        private Vector3 originPosition;
        
        public MyGrid(int width,int height)
        {
            originPosition = new Vector3(0, 0, 0);
            Vector3 startPosition;
            Vector3 upPosition;
            Vector3 rightPosition;
            gridArray = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    startPosition = originPosition + (Vector3.right) * i * CellSize + Vector3.up * j * CellSize;
                    upPosition = startPosition + Vector3.up * CellSize;
                    rightPosition = startPosition + Vector3.right * CellSize;
                    Debug.DrawLine(startPosition, upPosition, Color.red, 100f);
                    Debug.DrawLine(startPosition, rightPosition, Color.red, 100f);
                }
            }
            
        }
    }
}