using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class ObserVR_CustomEvent {
    public string timestamp;
    public string type;
    public string id;
    public string session;
    public string user;
    public List<ObserVR_KeyValuePair> parameters;
    private static List<ObserVR_CustomEvent> tempQueue;

    public ObserVR_CustomEvent(string type) {
        timestamp = ObserVR_Functions.GetCurrentTimeString();
        this.type = type;
        id = Guid.NewGuid().ToString();
        session = ObserVR_Session.Instance ? ObserVR_Session.Instance.sessionID : "";
        user = ObserVR_User.Instance ? ObserVR_User.Instance.userID : "";
        parameters = null;
    }

    public ObserVR_CustomEvent AddParameter(string key, string value) {
        if (parameters == null) {
            parameters = new List<ObserVR_KeyValuePair>();
        }
        parameters.Add(new ObserVR_KeyValuePair(key, value));
        return this;
    }

    public ObserVR_CustomEvent AddParameter(string key, float value) {
        if (parameters == null) {
            parameters = new List<ObserVR_KeyValuePair>();
        }
        parameters.Add(new ObserVR_KeyValuePair(key, value.ToString(CultureInfo.InvariantCulture)));
        return this;
    }

    public ObserVR_CustomEvent AddParameter(string key, double value) {
        if (parameters == null) {
            parameters = new List<ObserVR_KeyValuePair>();
        }
        parameters.Add(new ObserVR_KeyValuePair(key, value.ToString(CultureInfo.InvariantCulture)));
        return this;
    }

    public ObserVR_CustomEvent AddParameter(string key, int value) {
        if (parameters == null) {
            parameters = new List<ObserVR_KeyValuePair>();
        }
        parameters.Add(new ObserVR_KeyValuePair(key, value.ToString()));
        return this;
    }

    public ObserVR_CustomEvent AddParameter(string key, bool value) {
        if (parameters == null) {
            parameters = new List<ObserVR_KeyValuePair>();
        }
        parameters.Add(new ObserVR_KeyValuePair(key, value.ToString()));
        return this;
    }

    public ObserVR_CustomEvent AddParameter(string key, Vector3 value) {
        if (parameters == null) {
            parameters = new List<ObserVR_KeyValuePair>();
        }
        parameters.Add(new ObserVR_KeyValuePair(key, value.ToString("0.000")));
        return this;
    }

    public ObserVR_CustomEvent AddParameter(string key, string[] value) {
        if (parameters == null) {
            parameters = new List<ObserVR_KeyValuePair>();
        }
        parameters.Add(new ObserVR_KeyValuePair(key, ObserVR_Functions.ParseStringArray(value)));
        return this;
    }

    public void EndParameters() {
        if (ObserVR_EventHandler.Instance == null || !ObserVR_EventHandler.Instance.ObserVR_Initialized) {
            if (tempQueue == null) {
                tempQueue = new List<ObserVR_CustomEvent>();
            }
            tempQueue.Add(this);
        }
        else {
            ObserVR_EventHandler.Instance.AddEvent(this);
            if (tempQueue != null && tempQueue.Count > 0) {
                FlushTempQueue();
            }
        }
    }

    public void NoParameters() {
        if (ObserVR_EventHandler.Instance == null || !ObserVR_EventHandler.Instance.ObserVR_Initialized) {
            if (tempQueue == null) {
                tempQueue = new List<ObserVR_CustomEvent>();
            }
            tempQueue.Add(this);
        }
        else {
            ObserVR_EventHandler.Instance.AddEvent(this);
            if (tempQueue != null && tempQueue.Count > 0) {
                FlushTempQueue();
            }
        }
    }

    private static void FlushTempQueue() {
        foreach (ObserVR_CustomEvent tempEvent in tempQueue) {
            tempEvent.timestamp = ObserVR_Functions.GetCurrentTimeString();
            tempEvent.session = ObserVR_Session.Instance.sessionID;
            tempEvent.user = ObserVR_User.Instance.userID;
            ObserVR_EventHandler.Instance.AddEvent(tempEvent);
        }
        tempQueue.Clear();
    }
}
