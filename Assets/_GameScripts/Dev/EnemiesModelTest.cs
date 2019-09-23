using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class EnemiesModelTest : MonoBehaviour {

    public GameObject[] EnemyModels;

    public int Instances = 10;
    public float Space = 1;
    public Transform SpawnPoint;

    public bool TestAnims = true;

    private int _modelIndex;

    List<GameObject> _modelsRefs;

    void DestroyOld()
    {
        if (null == _modelsRefs ||
            _modelsRefs.Count == 0)
            return;

        foreach(var model in _modelsRefs)
        {
            Destroy(model);
        }
        _modelsRefs.Clear();
    }

    void SpawnModels()
    {
        DestroyOld();
        string mdlName = EnemyModels[_modelIndex].name;
        Debug.Log("Spawning : " + mdlName);
        Profiler.BeginSample("SpawnModels " + mdlName);
        int half = (Instances/2);
        for(int i = 0; i < Instances; ++i)
        {
            var mdlPf = EnemyModels[_modelIndex];
            var spawnPos = SpawnPoint.transform.position;
            spawnPos.x += (i - half) * Space;
            var mdlRef = Instantiate(mdlPf, spawnPos, Quaternion.identity);

            Health hp = mdlRef.GetComponentInChildren<Health>();
            if(null != hp)
                hp.enabled = false;

            if(TestAnims)
                mdlRef.AddComponent<TestModelsAnimations>();

            _modelsRefs.Add(mdlRef);
        }
        Profiler.EndSample();
        _modelIndex++;
        if (_modelIndex > EnemyModels.Length - 1)
            _modelIndex = 0;
    }

    private void Start()
    {
        _modelsRefs = new List<GameObject>();
        SpawnModels();
    }

    void Update () {
		if(Input.GetButtonDown("X") || Input.GetKeyDown(KeyCode.Space))
        {
            SpawnModels();
        }
	}
}
