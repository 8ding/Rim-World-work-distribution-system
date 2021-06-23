using System;
using System.Collections.Generic;
using TaskSystem;
using UnityEngine;

    public abstract class TaskBase
    {
        public TaskType taskType;
    }
    
    public class GatherResourceTask : TaskBase
    {
        public Vector3 StorePosition;
        public Action<int,GameHandler.ResourceManager> ResourceGrabed;
        public GameHandler.ResourceManager resourceManager;
    }

    public class WorkerMoveTask : TaskBase
    {
        public Vector3 Destination;
    }
    
    //NPC移动到并进行某种行为,该行为只有动画表现没有逻辑影响
    public class NPCMoveAndBahaveTask : TaskBase
    {
        public Vector3 Destination;

    }
