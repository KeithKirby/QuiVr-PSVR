using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

public class PS4UserManager : MonoBehaviour
{
#if UNITY_PS4
    void Start()
    {
    }

    PS4Input.LoggedInUser? FindUserByMoveHandles(int primary, int secondary)
    {
        if (primary != -1 && secondary != -1)
        {
            for (int i = 0; i < 4; ++i)
            {
                var user = PS4Input.GetUsersDetails(i);
                if (user.move0Handle == primary && user.move1Handle == secondary)
                    return user;
            }
        }
        return null;
    }

    private void CheckForUserChanged()
    {
        int activeUser = -1;
        for (int i = 0; i < 4; ++i)
        {
            var user = PS4Input.GetUsersDetails(i);
            if (user.primaryUser)
            {
                if (user.move0Handle != -1 && user.move1Handle != -1)
                {
                    activeUser = user.userId;
                }
            }
        }
        PrimaryUserId = activeUser;
    }

    public int PrimaryUserId
    {
        set
        {
            if (_primaryUserId != value)
            {
                _primaryUserId = value;
                DLog.Log(DLFilter.General, "Active user changed: '" + _primaryUserId + "'");
            }
        }
        get
        {
            return _primaryUserId;
        }
    }
    int _primaryUserId = -1;

#endif
}