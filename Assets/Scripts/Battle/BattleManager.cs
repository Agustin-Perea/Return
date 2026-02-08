using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static event Action<Fighter> OnPlayerTurnStarted;
    public static event Action OnPlayerActionSelected;

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

    void Start()
    {
        player = Instantiate(fighterPrefab).GetComponent<Fighter>();
        enemy = Instantiate(fighterPrefab).GetComponent<Fighter>();

        player.Init(playerSO, playerSpawn.position, true);
        enemy.Init(enemySO, enemySpawn.position, false);

        breakSystem = GetComponent<BreakSystem>();

        Debug.Log($"Start batlle \nPlayer HP: {player.currentHP}/{player.data.maxHP} || Enemy HP: {enemy.currentHP}/{enemy.data.maxHP} ");

        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        Debug.Log("Player's Turn");
        OnPlayerTurnStarted?.Invoke(player);
    }

    public void PlayerAttackPhysical()
    {
        Debug.Log("Player chose Physical Attack");
        OnPlayerActionSelected?.Invoke();

        ResolvePhysicalAttack(player, enemy);
        player.PlayAttackPhysical();

        CheckBattleEnd();

        OpenBreakWindow();
    }

    public void PlayerAttackMagic()
    {
        Debug.Log("Player chose Magic Attack");
        OnPlayerActionSelected?.Invoke();

        ResolveMagicAttack(player, enemy);
        player.PlayAttackMagic();

        CheckBattleEnd();

        OpenBreakWindow();
    }

    public void PlayerDefend()
    {
        Debug.Log("Player chose Defend");
        OnPlayerActionSelected?.Invoke();

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

    float GetPlayerHPPercent()
    {
        return (float)player.currentHP / player.data.maxHP;
    }

    void CheckBattleEnd()
    {
        if (!player.IsAlive)
        {
            player.PlayDeath();
            Debug.Log("PLAYER DEAD");
        }
        else if (!enemy.IsAlive)
        {
            enemy.PlayDeath();
            Debug.Log("ENEMY DEAD");
        }
    }
}
