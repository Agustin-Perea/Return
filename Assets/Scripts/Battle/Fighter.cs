using System;
using UnityEngine;

public class Fighter : MonoBehaviour
{
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

        if (isPlayer)
        {
            PlayIdleLeft();
        }
        else
        {
            PlayIdleRight();
        }
    }

    public void PlayIdleLeft()
    {
        animator?.SetFloat("Pos X", -1);
    }
    public void PlayIdleRight()
    {
        animator?.SetFloat("Pos X", 1);
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        OnDamage();
        Debug.Log($"{data.fighterName} received {dmg} damage. Current HP: {currentHP}");
    }
    public void OnDamage()
    {
        animator?.SetTrigger("Damage");
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
        throw new NotImplementedException();
    }

    public void PlayDeath()
    {
        animator?.SetTrigger("Fainted");
    }
}
