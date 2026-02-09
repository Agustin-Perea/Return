using System;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public event Action<float> OnTakeDamage;

    public FighterSO data;
    public int currentHP;

    private Animator animator;

    public bool IsAlive => currentHP > 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(FighterSO data, Vector2 position, bool isPlayer)
    {
        this.data = data;
        currentHP = data.maxHP;
        animator.runtimeAnimatorController = data.animator;

        transform.position = position;
    }

    public void TakeDamage(int dmg)
    {
        currentHP = Mathf.Max(currentHP - dmg, 0);
        OnDamage();
        Debug.Log($"{data.fighterName} received {dmg} damage. Current HP: {currentHP}");
    }
    public void OnDamage()
    {
        OnTakeDamage?.Invoke(GetHPPercent());
        animator?.SetTrigger("Damage");
    }

    public float GetHPPercent()
    {
        return (float)currentHP / data.maxHP;
    }

    public void PlayAttackPhysical()
    {
        animator?.SetTrigger("Attack");
    }

    public void PlayAttackMagic()
    {
        animator?.SetTrigger("Attack");
    }

    public void PlayDefend()
    {
        
    }

    public void PlayDeath()
    {
        animator?.SetTrigger("Fainted");
    }
}
