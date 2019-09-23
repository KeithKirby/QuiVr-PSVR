using UnityEngine;
using System.Collections;

public class EnemyDatabase : MonoBehaviour {

    public EnemyData[] Enemies;

    public static EnemyDatabase instance;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public EnemyData GetEnemy(EnemyType type)
    {
        return Enemies[(int)type];
    }

    public GameObject SpawnEnemy(EnemyType type, Transform spawn)
    {
        GameObject g = null;
        if (!PhotonNetwork.inRoom)
            g = (GameObject)Instantiate(Enemies[(int)type].prefab, spawn.position, spawn.rotation);
        else if (PhotonNetwork.isMasterClient)
            g = PhotonNetwork.InstantiateSceneObject("Enemies/" + Enemies[(int)type].prefab.name, spawn.position, spawn.rotation, 0, null);
        return g;
    }

    public bool isFlying(EnemyType t)
    {
        if (t == EnemyType.deathskull)
            return true;
        return false;
    }
}

[System.Serializable]
public class EnemyData
{
    public string Name;
    public GameObject prefab;
}

public enum EnemyType
{
    skeleton = 0,
    kobold,
    ghoul,
    orc,
    spider,
    deathskull,
    hellrot,
    ogre,
    beetle,
    cerberus,
    crasc,
    scorpion,
    lacodon,
    necromancer,
    goblin,
    giant,
    warlord,
    golem
}
