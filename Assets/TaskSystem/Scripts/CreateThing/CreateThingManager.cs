using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateThingManager : BaseManager<CreateThingManager>
{
    public static void Init()
    {
        EventCenter.Instance.AddEventListener<IArgs>(EventType.CreateUnit,CreateUint);
        EventCenter.Instance.AddEventListener<IArgs>(EventType.CreatMinePoit,createResourcePoint);
    }


    public static void CreateUint(IArgs _iArgs)
    {
        string name = (_iArgs as EventParameter<string,Vector3,int,float>).t1;
        Vector3 position = (_iArgs as EventParameter<string, Vector3, int, float>).t2;
        int ID = (_iArgs as EventParameter<string, Vector3, int, float>).t3;
        float Speed = (_iArgs as EventParameter<string, Vector3, int, float>).t4;
        GameObject unit = ResMgr.Instance.Load<GameObject>(name);
        unit.transform.position = position;
        UnitData unitData =  unit.AddComponent<UnitData>();
        unitData.CharacterId = ID;
        unitData.Speed = Speed;

        unit.AddComponent<MoveTransformVelocity>();
        unit.AddComponent<MovePositionPathFinding>(); 
        unit.AddComponent<UnitController>();

        UnitController unitController = unit.AddComponent<UnitController>();
        EventCenter.Instance.EventTrigger<IArgs>(EventType.UnitOccur,new EventParameter<UnitController>(unitController));

    }
    private static void createResourcePoint(IArgs _iArgs)
    {

        switch ((_iArgs as EventParameter<Vector3,ResourceType>).t2)
        {
            case ResourceType.Gold:
                PoolMgr.Instance.GetObj("MinePoint",(_o => { 
                    _o .transform.position = (_iArgs as EventParameter<Vector3, ResourceType>).t1;
                    new GameHandler.ResourceManager(_o .transform, (_iArgs as EventParameter<Vector3, ResourceType>).t2);
                }));
                break;
            case ResourceType.Wood:
                PoolMgr.Instance.GetObj("004_tree",(_o => { 
                    _o .transform.position = (_iArgs as EventParameter<Vector3, ResourceType>).t1;
                    new GameHandler.ResourceManager(_o .transform, (_iArgs as EventParameter<Vector3, ResourceType>).t2);
                }));
                break;
        }
    }

}
