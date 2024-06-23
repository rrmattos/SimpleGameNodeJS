using System.Collections.Generic;

public class StateManager<T>
{
    public List<T> CurrentState { get; private set; } = new List<T>();

    public void ChangeState(T _state)
    {
        CurrentState.Clear();
        CurrentState.Add(_state);
    }

    public void AddJointState(T _jointState)
    {
        if (!CurrentState.Contains(_jointState))
            CurrentState.Add(_jointState);
    }

    public void RemoveState(T _state)
    {
        if (CurrentState.Contains(_state))
            CurrentState.Remove(_state);
    }

    public bool Contains(T _state)
    {
        return CurrentState.Contains(_state);
    }
}
