using UnityEngine; // O el namespace que uses para Debug/Lógica

public class ExplorationState : BaseState<GameStateMachine.GameState>
{

    public ExplorationState(GameStateMachine.GameState key) : base(key)
    {
       
    }

    public override void OnEnter()
    {
        Debug.Log("Entrando en estado: EXPLORACIÓN. Activando controles de movimiento.");
        EventBus<PlayerEnableMovement>.Raise(new PlayerEnableMovement());
        EventBus<AddCombatCooldown>.Raise(new AddCombatCooldown());
    }

    public override void OnUpdate()
    {
       
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnExit()
    {
        Debug.Log("Saliendo de EXPLORACIÓN. Guardando posición actual.");
    }
}