using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Application", order = 1)]
public class AppValues : ScriptableObject
{
    public string NetworkVersion;
    public bool isDemo;
    public bool isBeta;
    public string CtoStr;
    public bool networkConnected;
    public bool serverOnline;
    public string AppServer;
    public bool cheated;
    public string LeaderboardID;
    public bool OfflineMode;
    public bool ArcadeMode;
    public bool AutoStart;
    public bool NoLobby;
    public Sprite ErrorIcon;
    public Sprite SuccessIcon;
    public ControllerType controls = ControllerType.ViveWand;
    public Platform platform;
}

public enum ControllerType
{
    ViveWand,
    OculusTouch,
    SteamVRKnuckles,
    WindowsMR,
    PSVR,
    MouseAndKeyboard,
    Xbox,
    Undefined
}

public enum Platform
{
    Steam,
    Omni,
    VirtualGate,
    UWP,
    Xbox,
    PS4,
    Mobile
}

