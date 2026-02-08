using System;
using Unity.VisualScripting;
using UnityEngine;

public class BreakSystem : MonoBehaviour
{
    public static event Action<float> OnBreakProgress;

    public float breakGauge = 0f;
    public float breakThreshold = 100f;
    public float breakWindowTime = 0.3f;
    public float breakGain = 20f;

    private float timer = 0f;
    private bool active = false;

    private Action onSuccess;
    private Action onFail;

    public void StartBreakWindow(Action onBreakSuccess, Action onBreakFail)
    {
        onSuccess = onBreakSuccess;
        onFail = onBreakFail;

        breakGauge = 0f;
        timer = breakWindowTime;
        active = true;
    }

    void Update()
    {
        if (!active) return;

        timer -= Time.deltaTime;

        if (breakGauge >= breakThreshold)
        {
            active = false;
            onSuccess?.Invoke();
        }

        if (timer <= 0f)
        {
            active = false;
            onFail?.Invoke();
        }
    }

    private void OnBreakInput()
    {
        breakGauge += breakGain;

        float normalized = breakGauge / breakThreshold;
        OnBreakProgress?.Invoke(normalized);
    }

    private void OnEnable()
    {
        BattleInput.OnBreakPressed += OnBreakInput;
    }

    private void OnDisable()
    {
        BattleInput.OnBreakPressed -= OnBreakInput;
    }
}
