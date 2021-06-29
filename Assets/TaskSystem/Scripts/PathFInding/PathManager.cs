using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum MoveDirection
{
    Right,
    UpRight,
    Up,
    UpLeft,
    Left,
    DownLeft,
    Down,
    DownRight,
    enumCount,
}
//路线及地图Grid管理模块
public class PathManager : BaseManager<PathManager>
{

    private static MyGrid<PathNode> _m_myGrid;
    private static PathFinding _m_pathFinding;

    public static void  Init()
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

    /// <summary>
    /// 获取给定位置所在网格的给定堆叠类型的剩余可放置内容物数量
    /// </summary>
    /// <param name="position">给定位置</param>
    /// <param name="_placedObjectType">给定的堆叠类型</param>
    /// <returns></returns>
    public int GetContentRoomLeft(Vector3 position, PlacedObjectType _placedObjectType)
    {
        PathNode pathNode = _m_pathFinding.GetNode(position);
        if(pathNode != null)
        {
            if(pathNode.placedObjectType == PlacedObjectType.none )
            {
                //返回读表获得的当前可放置物的最大数量，在这里先统一设为20
                return 20;
            }
            else if(pathNode.placedObjectType != _placedObjectType)
            {
                return 0;
            }
            else
            {
                return 20 - pathNode.placedObjectContentAMount;
            }
        }
        else
        {
            Debug.Log("网格错误");
            return 0;
        }
    }

    public int GetContentAmount(Vector3 _position, PlacedObjectType _placedObjectType)
    {
        PathNode pathNode = _m_pathFinding.GetNode(_position);
        if(pathNode != null)
        {
            if(pathNode.placedObjectType == PlacedObjectType.none )
            {
                return 0;
            }
            else if(pathNode.placedObjectType != _placedObjectType)
            {
                return 0;
            }
            else
            {
                return pathNode.placedObjectContentAMount;
            }
        }
        else
        {
            Debug.Log("网格错误");
            return 0;
        }
    }
    /// <summary>
    /// 增加位置所在网格的对应堆叠类型的内容物数量,返回剩余未放置的数量
    /// </summary>
    /// <param name="_position">位置</param>
    /// <param name="_placedObjectType">堆叠物类型</param>
    /// <param name="amount">增加数量</param>
    public int AddContentAmount(Vector3 _position, PlacedObjectType _placedObjectType, int amount)
    {
        PathNode pathNode = _m_pathFinding.GetNode(_position);
        int contentRoomLeft;
        if(pathNode != null)
        {
            contentRoomLeft= GetContentRoomLeft(_position, _placedObjectType);
            if(contentRoomLeft > 0)
            {
                if(amount <= contentRoomLeft)
                {
                    pathNode.placedObjectContentAMount += amount;
                    amount = 0;
                }
                else
                {
                    pathNode.placedObjectContentAMount += contentRoomLeft;
                    amount = amount - contentRoomLeft;
                }
                if(pathNode.placedObjectType == PlacedObjectType.none && _placedObjectType > PlacedObjectType.DividingLine)
                {
                    //构建搬运任务
                    TaskCenter.Instance.BuildTask(_position, GameObject.Find("Crate").transform.position,TaskType.CarryItem);
                }
                pathNode.placedObjectType = _placedObjectType;
                pathNode.PlacedObject =
                    CreateThingManager.Instance.ProducePlacedObject(pathNode.PlacedObject, _placedObjectType, pathNode.placedObjectContentAMount);
                pathNode.PlacedObject.transform.position = pathNode.worldPosition;
            }
        }
        else
        {
            Debug.Log("网格错误");
        }
        return amount;

    }
    /// <summary>
    /// 减少指定数量的给定位置所在网格的给定堆叠类型的内容物，返回未减少成功的数量
    /// </summary>
    /// <param name="_position">指定位置</param>
    /// <param name="_placedObjectType">指定堆叠类型</param>
    /// <param name="_amount">指定减少的数量</param>
    public int MinusContentAmount(Vector3 _position, PlacedObjectType _placedObjectType,  int _amount)
    {
        PathNode pathNode = _m_pathFinding.GetNode(_position);
        int aMount;
        if(pathNode != null)
        {
            aMount= GetContentAmount(_position, _placedObjectType);
            if(aMount > 0)
            {
                if(_amount < aMount)
                {
                    pathNode.placedObjectContentAMount -= _amount;
                    _amount = 0;
                }
                else
                {
                    pathNode.placedObjectContentAMount = 0;
                    pathNode.placedObjectType = PlacedObjectType.none;
                    _amount = _amount - aMount;
                }
                pathNode.PlacedObject =
                    CreateThingManager.Instance.ProducePlacedObject(pathNode.PlacedObject, pathNode.placedObjectType, pathNode.placedObjectContentAMount);
            }
        }
        else
        {
            Debug.Log("网格错误");
        }
        return _amount;
    }
    /// <summary>
    /// 检测给定位置是否有给定类型的堆叠物
    /// </summary>
    /// <param name="_placedObjectType">给定类型</param>
    ///<param name="_position">给定的位置</param>
    public bool IsHave(Vector3 _position,PlacedObjectType _placedObjectType)
    {
        PathNode pathNode = _m_pathFinding.GetNode(_position);
        if(pathNode != null)
        {
            if(pathNode.placedObjectType == PlacedObjectType.none)
            {
                return false;
            }
            else if(pathNode.placedObjectType != _placedObjectType)
            {
                return false;
            }
            return true;
        }
        Debug.Log("网格超出");
        return false;
    }
    /// <summary>
    /// 检测给定位置是否有任何堆叠物
    /// </summary>
    /// <param name="_position"></param>
    /// <returns></returns>
    public bool IsHaveAny(Vector3 _position)
    {
        PathNode pathNode = _m_pathFinding.GetNode(_position);
        if(pathNode != null)
        {
            if(pathNode.placedObjectType == PlacedObjectType.none)
            {
                return false;
            }
            return true;
        }
        Debug.Log("网格超出");
        return false;
    }
    /// <summary>
    /// 获取制定位置的堆叠物类型
    /// </summary>
    /// <param name="_position">指定位置</param>
    /// <returns></returns>
    public PlacedObjectType GetPlacedObjectType(Vector3 _position)
    {
        PathNode pathNode = _m_pathFinding.GetNode(_position);
        if(pathNode != null)
        {
            return pathNode.placedObjectType;
        }
        else
        {
            Debug.Log("网格错误");
            return PlacedObjectType.none;
        }
    }
    
}
