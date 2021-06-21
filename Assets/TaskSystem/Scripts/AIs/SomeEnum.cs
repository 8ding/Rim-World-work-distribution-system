public enum State
{
    WaitingForNextTask,
    ExecutingTask,
    TaskInterupt
}

public enum JobType
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