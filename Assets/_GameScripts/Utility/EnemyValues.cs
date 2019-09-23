using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyDatabase", order = 1)]
public class EnemyValues : ScriptableObject {
    public Enemy[] Enemies;
    [NotReorderable]
    public Enemy[] Bosses;
    public AnimationCurve ComboMultiplier;
    public AnimationCurve SpeedMultiplier;
    public float EnemyDensity = 1f;
    public float DifficultyTimeMult = 1f;
}

[System.Serializable]
public class Enemy
{
    public string Name;
    public CreatureType type;
    public CreatureSize size;
    public CreatureStatus status;
    //public int HP;
    [Header("Spawn")]
    public float DifficultyThreshold;
    [Range(0, 100)]
    public int Rarity;
    public int HalfThreshold = 100;
    public int StopThreshold = 999999;
    public int SPLimit;
    public int MaxNum;
    [Header("Misc")]
    public int pvpResource = 5;
    public float EnergyMult = -1;
    [Header("Model")]
    public GameObject prefab;
    public GameObject elitePrefab;
    public GameObject[] Weapons;
    public Color[] BaseColors;
    public Color[] DetailColors;
    public Color[] ArmorColors;
    public Vector2 SpeedRange;

    public override string ToString()
    {
        if (Name != null)
            return Name;
        return base.ToString();
    }
}

[System.Serializable]
public class WaveType
{
    public float DifficultyThreshold;
    public SpawnData[] Spawns;
}

[System.Serializable]
public class SpawnData
{
    public int EnemyID;
    [Range(0, 100)]
    public int Rarity;
    [Range(0, 100)]
    public int HalfThreshold = 100;
}

public enum CreatureType
{
    Walking,
    Stationary,
    Flying,
    Any
}

public enum CreatureStatus
{
    Standard,
    Miniboss,
    Giant
}

public enum CreatureSize
{
    Tiny,
    Small,
    Medium,
    Large,
    Giant,
}