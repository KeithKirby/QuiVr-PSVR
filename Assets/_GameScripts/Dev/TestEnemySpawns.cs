using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemySpawns : MonoBehaviour {

    [Range(0,100)]
    public int EnemiesCount;

    [Range(0,100)]
    public float SpawnDelay;

    int _currentEnemy;

    IEnumerator Spawn()
    {
        var enemyStream = GateManager.CurrentGate().Streamer;
        for (int i = 0; i < EnemiesCount; ++i)
        {
            var enemy = _currentEnemy;
            enemyStream.AddEnemyClose(enemy);
            yield return new WaitForEndOfFrame();
        }
        
        _currentEnemy++;
        if (_currentEnemy > EnemyDB.v.Enemies.Length - 1)
            _currentEnemy = 0;
    }

    void SpawnEnemies()
    {
        StartCoroutine("Spawn");
    }

    private void Start()
    {
        //SpawnEnemies();
    }

    private void Update()
    {
        if(Input.GetButtonDown("X"))
        {
            SpawnEnemies();
        }

        if(Input.GetButtonDown("Square"))
        {
            var enemyStream = GateManager.CurrentGate().Streamer;
            enemyStream.KillAll();
        }
    }
}
