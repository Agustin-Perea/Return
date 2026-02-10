using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class BattleManager : MonoBehaviour
{
    public static event Action<Fighter> OnPlayerTurnStarted;
    public static event Action<int, int> OnPlayerActionSelected;
    public static event Action<int> OnPlayerNavigate;
    public static event Action OnAttackSelected;

    [Header("Fighter Data")]
    public FighterSO playerSO;
    public EnemyGroup enemyGroupSO;

    [Header("Spawn Points")]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform[] enemySpawn;

    [SerializeField] private GameObject fighterPrefab;

    private Fighter player;
    private List<Fighter> enemyGroup = new List<Fighter>();

    private BreakSystem breakSystem;

    private int selectedActionIndex = 0;
    private int enemyTargetIndex = 0;

    private bool playerTurn;
    private bool isActionSelected;

    private List<Fighter> fightersTurnOrder => new List<Fighter>() { player }.Concat(enemyGroup).ToList().OrderByDescending(x => x.speed).ToList();
    private int currentTurnIndex = -1;

    void Start()
    {
        player = Instantiate(fighterPrefab).GetComponent<Fighter>();
        player.Init(playerSO, playerSpawn.position, 0, true);

        for(int i = 0; i < enemyGroupSO.enemies.Length; i++)
        {
            var enemySO = enemyGroupSO.enemies[i];
            var enemyInstance = Instantiate(fighterPrefab).GetComponent<Fighter>();
            enemyInstance.Init(enemySO.fighter, enemySpawn[enemySO.position].position,enemySO.position, false);
            enemyGroup.Add(enemyInstance);
        }

        breakSystem = GetComponent<BreakSystem>();

        BattleInput.OnNavigatePressed += BattleInput_OnNavigatePressed;
        BattleInput.OnConfirmPressed += BattleInput_OnConfirmPressed; ;

        Debug.Log($"Start batlle \nPlayer HP: {player.currentHP}/{player.data.maxHP}");

        StartNextTurn();
    }

    private void StartNextTurn()
    {
        currentTurnIndex++;
        if(currentTurnIndex >= fightersTurnOrder.Count) currentTurnIndex = 0;

        if (fightersTurnOrder[currentTurnIndex].isPlayer)
        {
            StartPlayerTurn(fightersTurnOrder[currentTurnIndex]);
        }
        else
        {
            StartEnemyTurn(fightersTurnOrder[currentTurnIndex]);
        }
    }

    private void BattleInput_OnConfirmPressed()
    {
        if(!playerTurn) return;

        if (!isActionSelected)
        {
            HandleActionSelected();
        }
        else
        {
            Fighter target = enemyGroup[enemyTargetIndex];

            if (selectedActionIndex == 0)
                PlayerAttackPhysical(target);
            else if (selectedActionIndex == 1)
                PlayerAttackMagic(target);

            OnAttackSelected?.Invoke();
            isActionSelected = false;
        }
    }

    private void BattleInput_OnNavigatePressed(int value)
    {
        if(!playerTurn) return;

        if (!isActionSelected)
        {
            SelectAcion(value);
        }
        else
        {
            SelectTarget(value);
        }
    }

    private void StartPlayerTurn(Fighter player)
    {
        if (!player.IsAlive) return;

        currentTurnIndex = fightersTurnOrder.IndexOf(player);
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

        if (selectedActionIndex != 2)
        {
            if (enemyGroup[enemyTargetIndex] == null || !enemyGroup[enemyTargetIndex].IsAlive)
            {
                SelectTarget(1);
            }
            OnPlayerActionSelected?.Invoke(selectedActionIndex, enemyGroup[enemyTargetIndex].PositionIndex);
            isActionSelected = true;
        }
        else
        {
            OnPlayerActionSelected?.Invoke(-1, -1);
            PlayerDefend();
        }

    }

    private void SelectTarget(int value)
    {
        int index = enemyTargetIndex;

        while (true)
        {
            index -= value;

            if (index < 0 || index >= enemyGroup.Count)
            {
                break;
            }
                

            if (enemyGroup[index] != null && enemyGroup[index].IsAlive)
            {
                enemyTargetIndex = index;
                break;
            }
        }

        Debug.Log($"Selected Target Index: {enemyTargetIndex}");
    }

    private void PlayerAttackPhysical(Fighter enemy)
    {
        playerTurn = false;
        Debug.Log("Player chose Physical Attack");

        ResolvePhysicalAttack(player, enemy);
        player.PlayAttackPhysical();

        CheckBattleEnd();

        OpenBreakWindow();
    }

    private void PlayerAttackMagic(Fighter enemy)
    {
        playerTurn = false;
        Debug.Log("Player chose Magic Attack");

        ResolveMagicAttack(player, enemy);
        player.PlayAttackMagic();

        CheckBattleEnd();

        OpenBreakWindow();
    }

    private void PlayerDefend()
    {
        playerTurn = false;
        Debug.Log("Player chose Defend");


        player.PlayDefend();

        OpenBreakWindow();
    }

    private void OpenBreakWindow()
    {
        Debug.Log("Opening Break Window");
        breakSystem.StartBreakWindow( OnBreakSuccess, OnBreakFail);
    }

    private void OnBreakSuccess()
    {
        Debug.Log("Break Success! Player gets another turn.");
        StartPlayerTurn(fightersTurnOrder.FirstOrDefault(x => x.isPlayer)); 
    }

    private void OnBreakFail()
    {
        Debug.Log("Break Failed! Enemy's turn.");
        StartNextTurn();
    }

    private void StartEnemyTurn(Fighter enemy)
    {
        if (!enemy.IsAlive) 
        {
            StartNextTurn();
            return;
        } 

        Debug.Log("Enemy's Turn");
        ResolvePhysicalAttack(enemy, player);
        enemy.PlayAttackPhysical();

        CheckBattleEnd();

        StartNextTurn();
    }

    private void ResolvePhysicalAttack(Fighter attacker, Fighter target)
    {
        int damage = Mathf.Max(1, attacker.data.attack - target.data.defense);
        target.TakeDamage(damage);
    }

    private void ResolveMagicAttack(Fighter attacker, Fighter target)
    {
        int damage = Mathf.Max(1, attacker.data.magic);
        target.TakeDamage(damage);
    }


    private void CheckBattleEnd()
    {
        if (!player.IsAlive)
        {
            Debug.Log("PLAYER DEAD");
            EndBattle(false);
        }
        else if(enemyGroup.All(e => !e.IsAlive))
        {
            Debug.Log("ENEMY DEAD");
            EndBattle(true);
        }
    }

    private void EndBattle(bool playerWin)
    {
        Debug.Log(playerWin ? "Player Win" : "Player Lose");
        StartCoroutine(LoadScene("BattleScene"));
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
