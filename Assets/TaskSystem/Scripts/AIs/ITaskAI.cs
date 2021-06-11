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
        void setUp(Woker worker); 
        void RequestNextTask();
    }
}