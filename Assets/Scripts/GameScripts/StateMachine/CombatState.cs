using UnityEngine; // O el namespace que uses para Debug/Lógica

public class CombatState : BaseState<GameStateMachine.GameState>
{

    public CombatState(GameStateMachine.GameState key) : base(key)
    {
       
    }

    public override void OnEnter()
    {
        Debug.Log("Entrando en estado: CombatState. Activando controles de movimiento.");
    }

    public override void OnUpdate()
    {
       
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnExit()
    {
        Debug.Log("Saliendo de CombatState. Guardando posición actual.");
    }
}