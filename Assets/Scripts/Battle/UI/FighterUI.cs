using System;
using UnityEngine;
using UnityEngine.UI;

public class FighterUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Fighter fighter;

    private void OnEnable()
    {
        fighter.OnTakeDamage += UpdateHealthBar;
    }

    private void OnDisable()
    {
        fighter.OnTakeDamage -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float value)
    {
        healthBar.transform.localScale = new Vector3(value, 1, 1);
    }
}
