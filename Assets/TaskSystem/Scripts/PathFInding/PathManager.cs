using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// 将物体设置在网格位置上
    /// </summary>
    /// <param name="setGameObject">需要被设置的游戏物体</param>
    /// <param name="currentPosition">游戏物体当前的位置</param>
    public void SetOnGrid(GameObject setGameObject)
    {
        Vector3 fixPosition = _m_pathFinding.GetNode(setGameObject.transform.position).worldPosition;
        setGameObject.transform.position = fixPosition;
    }
    
}
