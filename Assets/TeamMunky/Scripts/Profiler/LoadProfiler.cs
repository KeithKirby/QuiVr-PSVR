using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadProfiler : MonoBehaviour
{
    public class ProfileBlock
    {
        public long Start;
        public string Msg;
        public object[] Args;
    }

    static LoadProfiler _inst;
    System.Diagnostics.Stopwatch _stopwatch;
    
    // Use this for initialization
    void Start () {
        if (null == _inst)
        {
            _inst = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Debug.LogError("Multiple LoadProfiler instances created");
            DestroyImmediate(gameObject);
        }
	}

    void Init()
    {
        _stopwatch = new System.Diagnostics.Stopwatch();
        _stopwatch.Start();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene sceneA, Scene sceneB)
    {
        Log("Scene {0} -> {1}", sceneA.name, sceneB.name );        
    }

    static public ProfileBlock StartBlock(string msg, params object[] args)
    {
        if (null != _inst)
            return new ProfileBlock() { Start = _inst._stopwatch.ElapsedMilliseconds, Msg = msg, Args = args };
        else
            return null;
    }

    static public void EndBlock(ProfileBlock block)
    {
        if (null != _inst && null != block)
        {
            var prefix = string.Format("ProfileBlock({0} {1}): ", block.Start, (_inst._stopwatch.ElapsedMilliseconds - block.Start) / 1000.0f);
            Debug.LogFormat(prefix + block.Msg, block.Args);
        }
    }

    static public void Log(string msg, params object[] args)
    {
        if(null!=_inst)
            Debug.LogFormat(string.Format("LoadProfiler({0}): ", _inst._stopwatch.ElapsedMilliseconds / 1000.0f) + msg, args);
    }
}
