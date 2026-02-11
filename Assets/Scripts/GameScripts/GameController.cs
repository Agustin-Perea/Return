using UnityEngine;
using System.Collections;
public class GameController : MonoBehaviour
{
    
    public MovementComponent player;

    public GameStateMachine gameStateMachine;
    public float combatProbability = 0.01f;

    public float cooldownTime = 2f;

    EventBinding<AddCombatCooldown> addCombatCooldownEventBinding;

    void Awake()
    {
        gameStateMachine = GetComponent<GameStateMachine>();
    }
    void Start()
    {
        player.movementPerformed.AddListener(OnPlayerMove);
        cooldownTime = 0f;

        addCombatCooldownEventBinding = new EventBinding<AddCombatCooldown>(OnAddCombatCooldown);
        EventBus<AddCombatCooldown>.Register(addCombatCooldownEventBinding);

    }

    // Update is called once per frame
    void Update()
    {


        
    }
    void OnPlayerMove()
    {
        
        if (cooldownTime > 0)
        {
            cooldownTime -= Time.fixedDeltaTime;
            return;
        }
            

        Vector3 currentPosition = player.transform.position;
        float distanceMoved = Vector3.Distance(currentPosition, player.lastPosition);


        if (distanceMoved <= 0) return;


        float roll = Random.Range(0f, 1f);
        

        if (roll < combatProbability * distanceMoved)
        {
            StartCoroutine(EncounterSequence());
        }

    }

    void OnAddCombatCooldown()
    {
        cooldownTime = 2f;
    }

    IEnumerator EncounterSequence()
    {
        Debug.Log("¡Encuentro! Tiempo congelado.");

        EventBus<CameraEvent>.Raise(new CameraEvent());
        EventBus<PlayerDisabeMovement>.Raise(new PlayerDisabeMovement());

        yield return new WaitForSecondsRealtime(1.5f); 

        // 5. Cambiar el estado y mover cámara
        gameStateMachine.ChangeState(GameStateMachine.GameState.Combat);

        yield return new WaitForSecondsRealtime(1.5f); 
        
        gameStateMachine.ChangeState(GameStateMachine.GameState.Exploration);
        //cambiar posicion de la camara a la posicion del combate, o activar una camara de combate
    }
}
