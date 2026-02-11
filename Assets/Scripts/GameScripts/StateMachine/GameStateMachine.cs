using System;
using Unity.VisualScripting;

[Serializable]
public class GameStateMachine : StateMachine<GameStateMachine.GameState>
{
    public enum GameState
    {
        Exploration,
        Combat
    }

    

    void Awake()
    {
        statesDict.Add(GameState.Exploration,new ExplorationState(GameState.Exploration));
        statesDict.Add(GameState.Combat,new CombatState(GameState.Combat));
        currentState = statesDict[GameState.Exploration];
    }
    void Start()
    {
        
    }

}