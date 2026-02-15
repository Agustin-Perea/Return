using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class BattleManager : MonoBehaviour
{
    public static event Action<Fighter> OnPlayerTurnStarted;
    public static event Action<int, int> OnPlayerActionSelected;
    public static event Action<int> OnPlayerNavigate;
    public static event Action<int> OnPlayerSwitchTarget;
    public static event Action OnAttackSelected;


    [Header("Spawn Points")]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform[] enemySpawn;

    [SerializeField] private GameObject fighterPrefab;

    private Fighter player;
    private List<Fighter> enemyGroup = new List<Fighter>();

    private BreakSystem breakSystem;

    private int selectedActionIndex = 0;
    private int enemyTargetIndex = 1;

    private bool playerTurn;
    private bool isActionSelected;

    private List<Fighter> fightersTurnOrder => new List<Fighter>() { player }.Concat(enemyGroup).ToList().OrderByDescending(x => x.speed).ToList();
    private int currentTurnIndex = -1;

    private void OnEnable()
    {
        breakSystem = GetComponent<BreakSystem>();

        BattleInput.OnNavigatePressed += BattleInput_OnNavigatePressed;
        BattleInput.OnConfirmPressed += BattleInput_OnConfirmPressed;
    }
    public void StartBattle(EnemyGroup enemyGroupSO, FighterSO playerSO)
    {
        ResetBattle();

        player = Instantiate(fighterPrefab).GetComponent<Fighter>();
        player.Init(playerSO, playerSpawn.position, 0, true);

        for (int i = 0; i < enemyGroupSO.enemies.Length; i++)
        {
            var enemySO = enemyGroupSO.enemies[i];
            var enemyInstance = Instantiate(fighterPrefab).GetComponent<Fighter>();
            enemyInstance.Init(enemySO.fighter, enemySpawn[enemySO.position].position, enemySO.position, false);
            enemyGroup.Add(enemyInstance);
        }

        Debug.Log($"Start batlle \nPlayer HP: {player.currentHP}/{player.data.maxHP}");

        StartNextTurn();
    }

    private void StartNextTurn()
    {
        currentTurnIndex++;
        if (currentTurnIndex >= fightersTurnOrder.Count) currentTurnIndex = 0;

        if (fightersTurnOrder[currentTurnIndex].isPlayer)
        {
            StartPlayerTurn(fightersTurnOrder[currentTurnIndex]);
        }
        else
        {
            StartCoroutine(StartEnemyTurn(fightersTurnOrder[currentTurnIndex]));
        }
    }

    private void BattleInput_OnConfirmPressed()
    {
        if (!playerTurn) return;

        if (!isActionSelected)
        {
            HandleActionSelected();
        }
        else
        {
            Fighter target = enemyGroup[enemyTargetIndex];

            if (selectedActionIndex == 0)
                StartCoroutine(PlayerAttackPhysical(target));
            else if (selectedActionIndex == 1)
                StartCoroutine(PlayerAttackMagic(target));

            OnAttackSelected?.Invoke();
            isActionSelected = false;
        }
    }

    private void BattleInput_OnNavigatePressed(int value)
    {
        if (!playerTurn) return;

        if (!isActionSelected)
        {
            SelectAcion(value);
        }
        else
        {
            SelectTarget(value);
            OnPlayerSwitchTarget?.Invoke(enemyTargetIndex);
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
        if (!playerTurn) return;

        selectedActionIndex = Mathf.Clamp(selectedActionIndex - value, 0, 2);
        OnPlayerNavigate?.Invoke(selectedActionIndex);

        Debug.Log($"Selected Action Index: {selectedActionIndex}");
    }

    private void HandleActionSelected()
    {
        if (!playerTurn) return;

        if (selectedActionIndex != 2)
        {
            if (enemyGroup[enemyTargetIndex] == null || !enemyGroup[enemyTargetIndex].IsAlive)
            {
                enemyTargetIndex = enemyGroup.IndexOf(enemyGroup.FirstOrDefault(x => x != null && x.IsAlive));
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

    private IEnumerator PlayerAttackPhysical(Fighter enemy)
    {
        playerTurn = false;
        Debug.Log("Player chose Physical Attack");

        yield return player.MoveToTarget(player.transform, enemy.transform.position + Vector3.left);

        yield return ResolvePhysicalAttack(player, enemy);

        if(!CheckBattleEnd())
            StartNextTurn();
    }

    private IEnumerator PlayerAttackMagic(Fighter enemy)
    {
        playerTurn = false;
        Debug.Log("Player chose Magic Attack");

        yield return player.MoveToTarget(player.transform, enemy.transform.position + Vector3.left);

        yield return ResolveMagicAttack(player, enemy);

        if (!CheckBattleEnd())
            StartNextTurn();
    }

    private void PlayerDefend()
    {
        playerTurn = false;
        Debug.Log("Player chose Defend");


        player.PlayDefend();
    }

    Coroutine moveCoroutine;

    private void OnBreakSuccess()
    {
        Debug.Log("Break Success! Player gets another turn.");
        StopCoroutine(moveCoroutine);
        StartCoroutine(CancelEnemyTurn(fightersTurnOrder[currentTurnIndex]));
    }

    private void OnBreakFail()
    {
        Debug.Log("Break Failed! Enemy's turn.");
        StartCoroutine(ContinueEnemyTurn(fightersTurnOrder[currentTurnIndex]));
    }

    private IEnumerator StartEnemyTurn(Fighter enemy)
    {
        if (!enemy.IsAlive)
        {
            if (!CheckBattleEnd())
                StartNextTurn();
            yield break;
        }

        breakSystem.OpenBreakWindow(OnBreakSuccess, OnBreakFail);
        Debug.Log("Enemy's Turn");

        moveCoroutine = StartCoroutine(enemy.MoveToTarget(enemy.transform, player.transform.position + Vector3.right * 10, breakSystem.breakWindowTime));
    }

    private IEnumerator ContinueEnemyTurn(Fighter enemy)
    {
        yield return ResolvePhysicalAttack(enemy, player);

        if (!CheckBattleEnd())
            StartNextTurn();
    }

    private IEnumerator CancelEnemyTurn(Fighter enemy)
    {
        yield return enemy.MoveBack();
        StartNextTurn();
    }

    private IEnumerator ResolvePhysicalAttack(Fighter attacker, Fighter target)
    {
        yield return attacker.PlayAttackPhysical();

        int damage = Mathf.Max(1, attacker.data.attack - target.data.defense);
        target.TakeDamage(damage);

        yield return attacker.MoveBack();
    }

    private IEnumerator ResolveMagicAttack(Fighter attacker, Fighter target)
    {
        yield return attacker.PlayAttackMagic();

        int damage = Mathf.Max(1, attacker.data.magic);
        target.TakeDamage(damage);

        yield return attacker.MoveBack();
    }


    private bool CheckBattleEnd()
    {
        if (!player.IsAlive)
        {
            Debug.Log("PLAYER DEAD");
            EndBattle(false);
            return true;
        }
        else if (enemyGroup.All(e => !e.IsAlive))
        {
            Debug.Log("ENEMY DEAD");
            EndBattle(true);
            return true;
        }
        return false;
    }

    private void EndBattle(bool playerWin)
    {
        Debug.Log(playerWin ? "Player Win" : "Player Lose");
        StartCoroutine(LoadScene("BattleScene"));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(2f);

        StartCoroutine(ChangeToExploration());

    }
    private void OnDisable()
    {
        BattleInput.OnNavigatePressed -= SelectAcion;
        BattleInput.OnConfirmPressed -= HandleActionSelected;
    }


    public IEnumerator ChangeToExploration()
    {
        EventBus<TransitionCameraEvent>.Raise(new TransitionCameraEvent { animationName = "SmoothTransition" });
        yield return new WaitForSeconds(0.5f);
        EventBus<ActiveExplorationState>.Raise(new ActiveExplorationState());
    }
    public void ResetBattle()
    {
        Debug.Log("Resetting Battle...");

        if (player != null) Destroy(player.gameObject);

        foreach (var enemy in enemyGroup)
        {
            if (enemy != null) Destroy(enemy.gameObject);
        }

        enemyGroup.Clear();
        player = null;

        currentTurnIndex = -1;
        selectedActionIndex = 0;
        enemyTargetIndex = 1; 
        playerTurn = false;
        isActionSelected = false;
    }
}
