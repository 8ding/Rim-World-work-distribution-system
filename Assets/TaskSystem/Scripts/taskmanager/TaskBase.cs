using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaskSystem
{
    public abstract class TaskBase
    {
    
    }
    
    public class GatherResourceTask : TaskBase
    {
        public Vector3 StorePosition;
        public Action<int,GameHandler.ResourceManager> ResourceGrabed;
        public GameHandler.ResourceManager resourceManager;
    }

    public class GatherWoodTask : TaskBase
    {
        
    }
    
}