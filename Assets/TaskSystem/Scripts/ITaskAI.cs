namespace TaskSystem
{
    public interface ITaskAI
    {
        void setUp(IWorker worker, PL_TaskSystem<TaskBase> taskSystem); 
        void RequestNextTask();
    }
}