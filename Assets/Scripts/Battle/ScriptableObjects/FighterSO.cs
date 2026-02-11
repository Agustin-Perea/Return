using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Fighter SO")]
public class FighterSO : ScriptableObject
{
    [Header("Info")]
    public string fighterName;

    [Header("Stats")]
    public int maxHP;
    public int attack;
    public int magic;
    public int defense;

    [Header("Visual")]
    public RuntimeAnimatorController animator;
}

