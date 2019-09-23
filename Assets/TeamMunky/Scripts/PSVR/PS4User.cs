using Sony.NP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS4User
{
    public bool IsInitialUser = false;
    public Sony.NP.Core.UserServiceUserId UserId { get; set; }
    public bool HasPermission = false;
    public Sony.NP.Core.ReturnCodes ReturnCode;
    public int Age = 0;
    public bool ChatRestriction = false;
    public bool UGCRestriction = false;
}