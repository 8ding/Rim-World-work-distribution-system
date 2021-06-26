using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MoveDirection
{
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
    enumCount,
}
//路线及地图Grid管理模块
public class PathManager : BaseManager<PathManager>
{

    private MyGrid<PathNode> _m_myGrid;
    private PathFinding _m_pathFinding;

    public PathManager()
    {
        _m_myGrid = ResMgr.Instance.Load<GridSetting>("New Grid Setting").grid;
        _m_pathFinding = new PathFinding(_m_myGrid);
    }
    /// <summary>
    /// 自动寻路接口
    /// </summary>
    /// <param name="startPosition">起始位置</param>
    /// <param name="endPosition">目标位置</param>
    /// <returns></returns>
    public List<PathNode> findPath(Vector3 startPosition, Vector3 endPosition)
    {
        return _m_pathFinding.FindPath(startPosition, endPosition);
    }
    /// <summary>
    /// 获取一个位置所在的网格位置
    /// </summary>
    /// <param name="Position">需要获取网格位置的游戏物体</param>
    public Vector3 GetGridPosition(Vector3 Position)
    {
        return _m_pathFinding.GetNode(Position).worldPosition;
        
    }
    /// <summary>
    /// 获取传入位置移动一个网格的位置
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_moveDirection"></param>
    /// <returns></returns>
    public Vector3 GetOneOffsetPositon(Vector3 _position, MoveDirection _moveDirection)
    {
        XY xy = _m_myGrid.GetXY(_position);

        switch (_moveDirection)
        {
            case MoveDirection.Up:
                return _m_pathFinding.GetNode(xy.GetX(), xy.GetY() + 1).worldPosition; 
            case MoveDirection.Down:
                return _m_pathFinding.GetNode(xy.GetX(), xy.GetY() - 1).worldPosition;
            case MoveDirection.Left:
                return _m_pathFinding.GetNode(xy.GetX() - 1, xy.GetY()).worldPosition; 
            case MoveDirection.Right:
                return _m_pathFinding.GetNode(xy.GetX() + 1, xy.GetY()).worldPosition; 

            case MoveDirection.UpLeft:
                return _m_pathFinding.GetNode(xy.GetX() - 1, xy.GetY() + 1).worldPosition; 
            case MoveDirection.UpRight:
                return _m_pathFinding.GetNode(xy.GetX() + 1, xy.GetY() + 1).worldPosition; 
            case MoveDirection.DownLeft:
                return _m_pathFinding.GetNode(xy.GetX() - 1, xy.GetY() - 1).worldPosition; 
            case MoveDirection.DownRight:
                return _m_pathFinding.GetNode(xy.GetX() + 1, xy.GetY() - 1).worldPosition; 
            
        }
        return Vector3.zero;
    }
}
