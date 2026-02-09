using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ActionSelectionUI : MonoBehaviour
{
    public GameObject ActionSelectionUIObject;
    public GameObject[] attacksSelector;
    void OnEnable()
    {
        BattleManager.OnPlayerTurnStarted += Show;
        BattleManager.OnPlayerActionSelected += Hide;
        BattleManager.OnPlayerNavigate += (value) =>
        {
            for (int i = 0; i < attacksSelector.Length; i++)
            {
                if (attacksSelector[i] == null) continue;

                if (i == value)
                {
                    attacksSelector[i].SetActive(true);
                }
                else
                {
                    attacksSelector[i].SetActive(false);
                }
            }
        };

        Hide();
        attacksSelector.ToList().ForEach(selector => selector.SetActive(false));
        attacksSelector[0].SetActive(true);
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
