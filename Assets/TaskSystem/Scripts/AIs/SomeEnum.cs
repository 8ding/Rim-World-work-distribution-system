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
    enumcount,
}

public enum UnitType
{
    Worker,
    Npc,
    Player,
    enumcount
}