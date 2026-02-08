using UnityEngine;

public class ActionSelectionUI : MonoBehaviour
{
    public GameObject ActionSelectionUIObject;
    void OnEnable()
    {
        BattleManager.OnPlayerTurnStarted += Show;
        BattleManager.OnPlayerActionSelected += Hide;
    }

    void OnDisable()
    {
        BattleManager.OnPlayerTurnStarted -= Show;
        BattleManager.OnPlayerActionSelected -= Hide;
    }

    private void Show(Fighter fighter)
    {
        ActionSelectionUIObject.SetActive(true);
    }

    private void Hide()
    {
        ActionSelectionUIObject.SetActive(false);
    }
}
