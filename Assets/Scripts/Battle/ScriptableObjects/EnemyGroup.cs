using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Enemy Group SO")]
public class EnemyGroup : ScriptableObject
{
    [Header("Info")]
    public string fighterName;

    [Header("Group")]
    public EnemyGroupExtensions[] enemies;
}

[System.Serializable]
public class EnemyGroupExtensions
{
    public FighterSO fighter;
    public int position;
}
