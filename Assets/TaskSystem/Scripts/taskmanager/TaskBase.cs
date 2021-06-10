using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaskSystem
{
    public abstract class TaskBase
    {
    
    }
    public class GatherTask : TaskBase
    {
        public class GatherGold : GatherTask
        {
            public Vector3 GoldPosition;
            public Vector3 StorePosition;
            public Action<int,GameHandler.MineManager> GoldGrabed;
            public GameHandler.MineManager mineManager;
            public Action GoldDropde;
            public Transform goldTransform;
            public List<GameHandler.MineManager> mineManagerList;
            
        }
    }
}