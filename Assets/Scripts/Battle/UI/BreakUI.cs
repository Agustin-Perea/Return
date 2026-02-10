using UnityEngine;

public class BreakUI : MonoBehaviour
{
    void OnEnable()
    {
        BreakSystem.OnBreakProgress += OnBreak;
    }

    void OnDisable()
    {
        BreakSystem.OnBreakProgress -= OnBreak;
    }

    void OnBreak(float value)
    {
        // value va de 0 a 1
        // ejemplo:
        // scale
        // glow
        // shake
        // sonido
    }
}
