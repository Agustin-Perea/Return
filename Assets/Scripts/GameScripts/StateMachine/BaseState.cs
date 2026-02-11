
using System;
public abstract class BaseState<EState> where EState: Enum
{

    public BaseState(EState state_key)
    {
        StateKey = state_key;
    }
    public EState StateKey { get; private set; }

    public virtual void OnEnter()
    {
        
    }

    public virtual void OnExit()
    {
        
    }

    public virtual void OnUpdate()
    {
        
    }

    public virtual void OnFixedUpdate()
    {
        
    }
}
