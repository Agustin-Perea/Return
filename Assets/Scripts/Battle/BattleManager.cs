using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static event Action<Fighter> OnPlayerTurnStarted;
    public static event Action OnPlayerActionSelected;
    public static event Action<int> OnPlayerNavigate;

    [Header("Fighter Data")]
    public FighterSO playerSO;
    public FighterSO enemySO;

    [Header("Spawn Points")]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform enemySpawn;

    [SerializeField] private GameObject fighterPrefab;

    private Fighter player;
    private Fighter enemy;

    private BreakSystem breakSystem;

    private int selectedActionIndex = 0;
    private bool playerTurn;

    void Start()
    {
        player = Instantiate(fighterPrefab).GetComponent<Fighter>();
        enemy = Instantiate(fighterPrefab).GetComponent<Fighter>();

        player.Init(playerSO, playerSpawn.position, true);
        enemy.Init(enemySO, enemySpawn.position, false);

        breakSystem = GetComponent<BreakSystem>();

        BattleInput.OnNavigatePressed += SelectAcion;
        BattleInput.OnConfirmPressed += HandleActionSelected;

        Debug.Log($"Start batlle \nPlayer HP: {player.currentHP}/{player.data.maxHP} || Enemy HP: {enemy.currentHP}/{enemy.data.maxHP} ");

        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        if (!player.IsAlive) return;

        Debug.Log("Player's Turn");
        playerTurn = true;
        OnPlayerTurnStarted?.Invoke(player);
    }

    private void SelectAcion(int value)
    {
        if(!playerTurn) return;

        selectedActionIndex = Mathf.Clamp(selectedActionIndex - value, 0, 2);
        OnPlayerNavigate?.Invoke(selectedActionIndex);

        Debug.Log($"Selected Action Index: {selectedActionIndex}");
    }
    
    private void HandleActionSelected()
    {
        if(!playerTurn) return;

        if (selectedActionIndex == 0)
        {
            PlayerAttackPhysical();
        }
        else if(selectedActionIndex == 1)
        {
            PlayerAttackMagic();
        }
        else if(selectedActionIndex == 2)
        {
            PlayerDefend();
        }


        OnPlayerActionSelected?.Invoke();

        playerTurn = false;
    }

    public void PlayerAttackPhysical()
    {
        Debug.Log("Player chose Physical Attack");

        ResolvePhysicalAttack(player, enemy);
        player.PlayAttackPhysical();

        CheckBattleEnd();

        OpenBreakWindow();
    }

    public void PlayerAttackMagic()
    {
        Debug.Log("Player chose Magic Attack");

        ResolveMagicAttack(player, enemy);
        player.PlayAttackMagic();

        CheckBattleEnd();

        OpenBreakWindow();
    }

    public void PlayerDefend()
    {
        Debug.Log("Player chose Defend");


        player.PlayDefend();

        OpenBreakWindow();
    }

    void OpenBreakWindow()
    {
        Debug.Log("Opening Break Window");
        breakSystem.StartBreakWindow( OnBreakSuccess, OnBreakFail);
    }

    void OnBreakSuccess()
    {
        Debug.Log("Break Success! Player gets another turn.");
        StartPlayerTurn(); 
    }

    void OnBreakFail()
    {
        Debug.Log("Break Failed! Enemy's turn.");
        StartEnemyTurn();
    }

    void StartEnemyTurn()
    {
        if (!enemy.IsAlive) return;

        Debug.Log("Enemy's Turn");
        ResolvePhysicalAttack(enemy, player);
        enemy.PlayAttackPhysical();

        CheckBattleEnd();

        StartPlayerTurn();
    }

    void ResolvePhysicalAttack(Fighter attacker, Fighter target)
    {
        int damage = Mathf.Max(1, attacker.data.attack - target.data.defense);
        target.TakeDamage(damage);
    }

    void ResolveMagicAttack(Fighter attacker, Fighter target)
    {
        int damage = Mathf.Max(1, attacker.data.magic);
        target.TakeDamage(damage);
    }


    void CheckBattleEnd()
    {
        if (!player.IsAlive)
        {
            player.PlayDeath();
            Debug.Log("PLAYER DEAD");

            StartCoroutine(LoadScene("BattleScene"));
        }
        else if (!enemy.IsAlive)
        {
            enemy.PlayDeath();
            Debug.Log("ENEMY DEAD");

            StartCoroutine(LoadScene("BattleScene"));
        }
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }
    private void OnDisable()
    {
        BattleInput.OnNavigatePressed -= SelectAcion;
        BattleInput.OnConfirmPressed -= HandleActionSelected;
    }
}
