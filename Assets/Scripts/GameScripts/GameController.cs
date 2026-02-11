using UnityEngine;
using System.Collections;
public class GameController : MonoBehaviour
{

    public MovementComponent player;

    public GameStateMachine gameStateMachine;
    public float combatProbability = 0.01f;

    public float cooldownTime = 2f;

    EventBinding<AddCombatCooldown> addCombatCooldownEventBinding;
    EventBinding<ActiveExplorationState> onActiveExplorationStateEventBinding;

    public GameObject explorationCam;
    public GameObject combatCam;


    void Awake()
    {
        gameStateMachine = GetComponent<GameStateMachine>();

    }
    void Start()
    {
        player.movementPerformed.AddListener(OnPlayerMove);


        addCombatCooldownEventBinding = new EventBinding<AddCombatCooldown>(OnAddCombatCooldown);
        onActiveExplorationStateEventBinding = new EventBinding<ActiveExplorationState>(OnactiveExplorationState);
        EventBus<AddCombatCooldown>.Register(addCombatCooldownEventBinding);
        EventBus<ActiveExplorationState>.Register(onActiveExplorationStateEventBinding);

    }
    void OnactiveExplorationState()
    {
        explorationCam.SetActive(true);
        combatCam.SetActive(false);

        gameStateMachine.ChangeState(GameStateMachine.GameState.Exploration);
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

        EventBus<TransitionCameraEvent>.Raise(new TransitionCameraEvent { animationName = "CombatTransition" });
        EventBus<PlayerDisabeMovement>.Raise(new PlayerDisabeMovement());

        yield return new WaitForSecondsRealtime(1.5f);

        gameStateMachine.ChangeState(GameStateMachine.GameState.Combat);
        SwitchToCombat();
    }

    public void SwitchToCombat()
    {
        explorationCam.SetActive(false);
        combatCam.SetActive(true);
    }


    void OnEnable()
    {
        addCombatCooldownEventBinding = new EventBinding<AddCombatCooldown>(OnAddCombatCooldown);
        onActiveExplorationStateEventBinding = new EventBinding<ActiveExplorationState>(OnactiveExplorationState);
        EventBus<AddCombatCooldown>.Register(addCombatCooldownEventBinding);
        EventBus<ActiveExplorationState>.Register(onActiveExplorationStateEventBinding);
    }
    void OnDisable()
    {
        EventBus<AddCombatCooldown>.Deregister(addCombatCooldownEventBinding);
        EventBus<ActiveExplorationState>.Deregister(onActiveExplorationStateEventBinding);
    }
}
