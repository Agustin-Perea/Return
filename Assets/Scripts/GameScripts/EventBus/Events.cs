public interface IEvent { }



//eventos concretos de cada tipo de evento, pueden tener datos asociados o no, dependiendo de la necesidad. Se pueden crear tantos eventos como se necesiten, y cada uno puede ser manejado por diferentes sistemas o componentes del juego.

//cada interfaz particular puede tener un enlace de eventos(EventBinding) asociado, que se registra en el EventBus para ese tipo de evento. Cuando se lanza un evento, el EventBus se encarga de invocar los métodos registrados en los enlaces de eventos correspondientes a ese tipo de evento.
public struct CameraEvent : IEvent { }
public struct TransitionCameraEvent : IEvent
{
    public string animationName;
}

public struct AddCombatCooldown : IEvent { }
public struct PlayerDisabeMovement : IEvent
{
    
}
public struct PlayerEnableMovement : IEvent
{
    
}

public struct ActiveExplorationState : IEvent { }