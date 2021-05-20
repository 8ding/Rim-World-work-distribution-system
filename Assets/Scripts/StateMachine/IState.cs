
using UnityEngine.UIElements;

public interface IState
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
}
