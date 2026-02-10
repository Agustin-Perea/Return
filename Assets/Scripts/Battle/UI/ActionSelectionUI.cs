using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ActionSelectionUI : MonoBehaviour
{
    public GameObject ActionSelectionUIObject;
    public GameObject[] attacksSelector;
    public GameObject[] targetSelector;
    void OnEnable()
    {
        BattleManager.OnPlayerTurnStarted += ShowActionSelection;
        BattleManager.OnPlayerActionSelected += BattleManager_OnPlayerActionSelected; ;
        BattleManager.OnAttackSelected += HideTargetSelection;
        BattleManager.OnPlayerNavigate += SwitchActionSelector;

        HideActionSelection();
        HideTargetSelection();
        attacksSelector.ToList().ForEach(selector => selector.SetActive(false));
        attacksSelector[0].SetActive(true);
    }

    private void BattleManager_OnPlayerActionSelected(int action, int target)
    {
        HideActionSelection();

        SwitchTargetSelection(action, target);
    }

    void OnDisable()
    {
        BattleManager.OnPlayerTurnStarted -= ShowActionSelection;
        BattleManager.OnPlayerActionSelected -= BattleManager_OnPlayerActionSelected;
        BattleManager.OnAttackSelected -= HideTargetSelection;
        BattleManager.OnPlayerNavigate -= SwitchActionSelector;
    }

    private void ShowActionSelection(Fighter fighter)
    {
        ActionSelectionUIObject.SetActive(true);
    }

    private void SwitchTargetSelection(int action, int target)
    {
        if(action < 0) return;

        HideTargetSelection();
        targetSelector[target].SetActive(true);
    }

    private void HideActionSelection()
    {
        ActionSelectionUIObject.SetActive(false);
    }

    private void HideTargetSelection()
    {
        targetSelector.ToList().ForEach(selector => selector.SetActive(false));
    }

    private void SwitchActionSelector(int value)
    {
        attacksSelector.ToList().ForEach(selector => selector.SetActive(false));
        attacksSelector[value].SetActive(true);
    }
}
