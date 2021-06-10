namespace TaskSystem
{
    public enum State
    {
        WaitingForNextTask,
        ExecutingTask,
    }
    public interface ITaskAI
    {
        void setUp(IWorker worker, PL_TaskSystem<TaskBase> taskSystem); 
        void RequestNextTask();
    }
}