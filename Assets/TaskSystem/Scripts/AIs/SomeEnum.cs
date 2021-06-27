public enum State
{
    WaitingForNextTask,
    ExecutingTask,
    Wondering,
}

//工人任务类型
public enum TaskType
{
    GatherResource,
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