using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class BattleInput : MonoBehaviour
{
    public static event Action OnConfirmPressed;
    public static event Action OnBreakPressed;
    public static BattleInput Instance { get; private set; }

    private BattleInputActions input;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        input = new BattleInputActions();
    }

    private void OnEnable()
    {
        input.Battle.Enable();

        input.Battle.Confirm.performed += OnConfirm;
        input.Battle.Break.performed += OnBreak;
    }

    private void OnDisable()
    {
        input.Battle.Confirm.performed -= OnConfirm;
        input.Battle.Break.performed -= OnBreak;

        input.Battle.Disable();
    }

    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        OnConfirmPressed?.Invoke();
    }

    private void OnBreak(InputAction.CallbackContext ctx)
    {
        OnBreakPressed?.Invoke();
    }
}
