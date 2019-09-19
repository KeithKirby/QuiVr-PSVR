using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PS4MatchmakingHelperProxy : MonoBehaviour
{
    public void SendInvite()
    {
        var helper = GameObject.FindObjectOfType<PS4MatchmakingHelper>();
        helper.Invite();
    }
}