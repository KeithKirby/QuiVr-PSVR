using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum DeviceType // Please keep this in sync with DeviceTypeFlags
{
    htcvive,
    oculus,
    psvr,
    novr
}

public enum DeviceTypeFlags
{
    htcvive = 1<<DeviceType.htcvive,
    oculus = 1<<DeviceType.oculus,
    psvr = 1<<DeviceType.psvr,
    novr = 1<< DeviceType.novr,
}

public enum GameScene
{
    ModernBar,
    SaloonBar,
    RooftopBar
}

public enum MultiPlayerRole
{
    server,
    client
}
public enum NetworkType
{
    Photon,
    Zeus
}

public static class GameGlobeData {

    static public string GetSceneName(GameScene scene)
    {
        switch (scene)
        {
            case GameScene.ModernBar:
                return "SP_ModernBar";
            case GameScene.SaloonBar:
                return "SP_SaloonBar";
            case GameScene.RooftopBar:
                return "SP_RooftopBar";
            default:
                throw new System.Exception("Unknown scene:" + scene);
        }
    }

    public static float planeLevel = 1.4f;
    public static bool spectate = false;
    public static float playAreaScale
    {
        get { return _playAreaScale; }
        set
        {
            _playAreaScale = value;
        }
    }
    static float _playAreaScale = 3;
    //public static float playerHeight = 5;
    public static float controllerSeparation = 5;
    public static bool isNetwork = false;
    public static bool bodyIsDisplayed = false;
    public static bool networkInitialized = false;
    public static bool isThirdPersonView = false;
    public static bool isRunningLowFPS = false;
    public static bool isGameFinished = false;
    public static bool isGameManagerReady = false;
    public static int currentChalkboardIndex = 0;
    public static bool enableMovement = true;
#if UNITY_PS4
    public static DeviceType deviceType = DeviceType.psvr;
#elif OPENVR        
    public static DeviceType deviceType = DeviceType.htcvive;
#elif OCULUS
    public static DeviceType deviceType = DeviceType.oculus;
#else // No VR
    public static DeviceType deviceType = DeviceType.novr;
#endif    
    public static NetworkType networkType = NetworkType.Zeus;
    public static GameScene currentScene = GameScene.ModernBar;
    public static string currentConnectHost = "";
    public static MultiPlayerRole multiPlayerRole = MultiPlayerRole.server;
    //public static Vector3 playerAreaOriginalPosition = new Vector3(0, 1.38f, -1.4f);
    //public static Vector3 playerKOPosition;
    //public static Quaternion playerAreaOriginalRotation = Quaternion.identity;
    //public static Vector3 playerAreaOutSidePosition = new Vector3(0, 1.38f, -1.4f);
    //public static Quaternion playerAreaOutSideRotation = Quaternion.identity;
    public static float fixedTime = 0.01f;
    public static float maxPinWeight = 1;
    public static string playerName = "";
    public static string playerType = "";
    public static string versionNum = "201710141122";
    
#if UNITY_PS4
    public static int currentRound = 0;
    public static int currentRound1 = 0;
    public static int currentRound2 = 0;
    public static int limitActiveCharacterCount = 4;
    public static int limitFightCharacterCount = 4;

    public static int[][] modernBarRounds =
    {
        new int [] {0},
        new int [] {1,2},
        new int [] {3,4,5},
        new int [] {0,1,6,7},
        new int [] {2,3,10,11},
        new int [] {0,6,7,8},
        new int [] {4,5,9,11}
    };

    public static int[][] saloonBarRounds =
    {
        new int [] {0},
        new int [] {1,2},
        new int [] {3,4,5},
        new int [] {0,1,6,7},
        new int [] {2,3,9,10},
        new int [] {0,6,7,8},
        new int [] {4,5,9,11}
    };

    public static int[][] rooftopBarRounds =
    {
        new int [] {0},
        new int [] {1,2},
        new int [] {3,4,5},
        new int [] {0,1,6,7},
        new int [] {2,3,9,13},
        new int [] {0,6,7,11},
        new int [] {4,5,8,13}
    };

#else
        public static int limitActiveCharacterCount = 15;
        public static int limitFightCharacterCount = 4;
#endif
    
    public static bool checkEnemyIsInRound(int id)
    {
        return false;
    }
}
