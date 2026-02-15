using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public event Action<float> OnTakeDamage;

    public FighterSO data;
    public int currentHP;
    public int speed;
    public bool isPlayer;

    private Animator animator;
    private Vector2 initialPosition;

    public bool IsAlive => currentHP > 0;
    public int PositionIndex { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(FighterSO data, Vector2 position, int positionIndex, bool isPlayer)
    {
        this.data = data;
        currentHP = data.maxHP;
        PositionIndex = positionIndex;
        this.isPlayer = isPlayer;
        animator.runtimeAnimatorController = data.animator;

        initialPosition = position;
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

        if (currentHP <= 0)
        {
            StartCoroutine(PlayDeath());
        }
    }

    public float GetHPPercent()
    {
        return (float)currentHP / data.maxHP;
    }

    public IEnumerator MoveToTarget(Transform mover, Vector3 targetPosition, float duration = 0.7f)
    {
        Vector3 startPosition = mover.position;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            mover.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        mover.position = targetPosition;
    }

    public IEnumerator MoveBack()
    {
        yield return MoveToTarget(transform, initialPosition);
    }

    public IEnumerator PlayAttackPhysical()
    {
        animator?.SetTrigger("Attack");

        yield return WaitForAnimation();
    }

    public IEnumerator PlayAttackMagic()
    {
        animator?.SetTrigger("Attack");

        yield return WaitForAnimation();
    }

    public IEnumerator PlayDefend()
    {

        yield return WaitForAnimation();
    }

    public IEnumerator PlayDeath()
    {
        animator?.SetTrigger("Fainted");
        yield return WaitForAnimation();
    }

    private IEnumerator WaitForAnimation()
    {
        yield return null;

        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        float duration = clipInfo[0].clip.length / animator.speed;

        yield return new WaitForSeconds(duration);
    }
}
