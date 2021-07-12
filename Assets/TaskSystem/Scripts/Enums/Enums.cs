

public enum State
{
    WaitingForNextTask,
    ExecutingTask,
    Wondering,
}

//工人任务类型
public enum TaskType
{
    GatherGold,
    GatherWood,
    MakeThing,
    GoToPlace,
    CarryItem,
    enumcount,
}

public enum UnitType
{
    Worker,
    Npc,
    Player,
    enumcount
}
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

public enum ItemType
{
    Seed,
    Gold,
    Wood,
    Resource,
    Watering_tool,
    Hoeing_tool,
    Chopping_tool,
    Breaking_tool,
    Reaping_tool,
    Colleting_tool,
    Reapable_Scenary,
    Furniture,
    enumcount,
    None,
}

public enum ItemState
{
    OnGround,
    OnUnit,
}
