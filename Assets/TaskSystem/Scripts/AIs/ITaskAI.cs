using System.Collections.Generic;

namespace TaskSystem
{
    public enum State
    {
        WaitingForNextTask,
        ExecutingTask,
    }
    public interface ITaskAI
    {
        void setUp(IWorker worker); 
        void RequestNextTask();
    }
}