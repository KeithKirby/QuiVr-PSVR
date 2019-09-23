using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
/// <summary>
/// Utility which brings together all the different online checks
/// </summary>

public enum PS4ConnectionState
{
    Ok,
    ErrorAlreadyChecking,
    ErrorCheckAvailability,
    ErrorPhotonCouldNotConnect,
    ErrorCheckPlusEntitlement,    
}

public class PS4ConnectionCheck : Photon.MonoBehaviour
{

    PS4ConnectionCheck(Action<PS4ConnectionState> onComplete)
    {
        _onComplete = onComplete;
    }

    Action<PS4ConnectionState> _onComplete;
    bool _photonOk = false;
    bool _plusOk = false;

    void OnPhotonConnect(bool ok)
    {
        if (ok)
        {
            _photonOk = true;
            CheckFinished();
        }
        else
        {
            _onComplete(PS4ConnectionState.ErrorPhotonCouldNotConnect);
        }
    }

    void OnPlusConnect(bool ok)
    {
        if (ok)
        {
            _plusOk = true;
            CheckFinished();
        }
        else
        {
            _onComplete(PS4ConnectionState.ErrorCheckPlusEntitlement);
        }
    }

    void CheckFinished()
    {
        if(_photonOk && _plusOk)
        {
            _onComplete(PS4ConnectionState.Ok);
        }
    }

    static public void CheckConnection(Action<PS4ConnectionState> onComplete)
    {
        if (!PS4Plus.Inst.IsChecking)
        {
            var connectionCheck = new PS4ConnectionCheck(onComplete);
            PS4Plus.Inst.CheckAvailabilityAll(
                (ok) =>
                {
                    if (ok)
                    {
                        PS4Photon.instance.DoConnect((photonOk) => { connectionCheck.OnPhotonConnect(photonOk); });
                        PS4Plus.Inst.CheckPlus((plusOk) => { connectionCheck.OnPlusConnect(plusOk); });

                    }
                    else
                    {
                        onComplete(PS4ConnectionState.ErrorCheckAvailability);
                    }
                });
        }
        else
        {
            onComplete(PS4ConnectionState.ErrorAlreadyChecking);
        }
    }
}