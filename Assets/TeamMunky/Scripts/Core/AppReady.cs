using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class used to notify game objects when app is ready to start
public class AppReady : MonoBehaviour {

    public delegate void OnAppReady();

    struct ReadyCallback
    {
        public Action Callback;
        public string Tag;
    }

    static bool _appReady = false;
    static List<ReadyCallback> _onReady = new List<ReadyCallback>();
    static int _waitingForComponents = 0;

    // Call action when globals have been initialised
    static public void RequestReadyCallback(Action cb, string tag = "Unnamed")
    {
        _onReady.Add(new ReadyCallback() { Callback = cb, Tag = tag });
    }
    static public void cleanCallBack()
    {
        _onReady.Clear();
    }

    static public void BlockReady()
    {
        if (_appReady == true)
            throw new Exception("Block startup called when game already started");
        ++_waitingForComponents;
    }

    static public void StartupReady()
    {
        if (_appReady != false)
            throw new Exception("StartupReady called when game ");
        --_waitingForComponents;
    }

    void Update()
    {
        UpdateReadyCallbacks();
    }

    void UpdateReadyCallbacks()
    {
        if(_appReady == false &&
            _waitingForComponents == 0)
        {
            Debug.Log("ready2");
            _appReady = true;
        }
        if(_appReady && _onReady.Count>0)
        {
            Debug.Log("ready4");
            foreach (var r in _onReady)
            {
               // Debug.Log("AppReady Callback for " + r.Tag);
                r.Callback();
            }
            _onReady.Clear();
        }
    }
}
