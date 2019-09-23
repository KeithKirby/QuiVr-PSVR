using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
[RequireComponent (typeof(PhotonView))]
public class PlayerSync : MonoBehaviour
{
    public Text PlayerName;

    public float InterpRate;

    public GameObject ArrowPrefab;
    public GameObject Dummy;
    public GameObject Real;
    //public GameObject Talents;

    public Transform RealHead;
    public Transform RealHandLeft;
    public Transform RealHandRight;

    KnucklesHandControl RHKnuckle;
    KnucklesHandControl LHKnuckle;
    bool fingerTrack;
    float[] rhK;
    float[] lhK;

    public Transform DummyHead;
    public Transform DummyHandLeft;
    public Transform DummyHandRight;

    public WingObject DummyWings;

    Vector3 HeadPos;
    Vector3 LHandPos;
    Vector3 RHandPos;

    Quaternion HeadRot;
    Quaternion LHandRot;
    Quaternion RHandRot;

    public Transform RealBow;
    public Transform DummyBow;

    public Transform RealQuiver;
    public Transform DummyQuiver;

    public GameObject DummyArrowNocked;
    public ChestFollow DummyChest;
    public TeleporterParticles DummyTParticles;

    Vector3 BowPos;
    Vector3 QuiverPos;

    Quaternion BowRot;
    Quaternion QuiverRot;

    HandAnim lhDummyAnim;
    HandAnim rhDummyAnim;
    HandAnim lhRealAnim;
    HandAnim rhRealAnim;
    float lhGrip;
    float rhGrip;


    public BowAnimation bowRealAnim;
    BowAnimation bowDummyAnim;
    float bowPull;

    public BowAim bowAim;

    bool arrowNocked;

    public bool selfMute;
    public bool muted;

    AudioSource src;

    public bool active = true;

    bool dead;
    bool deadUpd;
    public PlayerLife plrLife;
    public EtherialSwap dummyDead;

    public GameObject LeftDummyOrb;
    public GameObject RightDummyOrb;

    DummyArmor armor;

    public static PlayerSync myInstance;
    public static List<PlayerSync> Others;

    [HideInInspector]
    public bool bubbleOn;
    [HideInInspector]
    public Outfit networkOutfit;
    bool mine;
    PhotonVoiceRecorder rec;

    #region Base

    void Awake()
    {
        if (Others == null)
            Others = new List<PlayerSync>();
        lhDummyAnim = DummyHandRight.GetComponentInChildren<HandAnim>();
        rhDummyAnim = DummyHandLeft.GetComponentInChildren<HandAnim>();
        bowDummyAnim = DummyBow.GetComponent<BowAnimation>();
        src = GetComponent<AudioSource>();
        armor = GetComponentInChildren<DummyArmor>();
        RHKnuckle = RealHandRight.GetComponentInParent<KnucklesHandControl>();
        LHKnuckle = RealHandLeft.GetComponentInParent<KnucklesHandControl>();
        dummyArmor = GetComponentInChildren<DummyArmor>();
        if (PhotonNetwork.inRoom)
        {            
            PhotonView photonView = GetPhotonView();
            if (photonView.isMine)
            {
                //GetComponentInChildren<MainMenu>().MultiPlayer();
                mine = true;
                //PlayerName.text = "";
                DisplayName();
                rec = GetComponent<PhotonVoiceRecorder>();
                rec.enabled = true;
                GetComponent<PhotonVoiceSpeaker>().enabled = false;
                myInstance = this;                
            }
            else
            {
                if (null != photonView.owner)
                {
                    mine = false;
                    DisplayName();
                    Others.Add(this);
                    InvokeRepeating("DisplayName", 0.5f, 0.503f);
                }
            }
            Debug.LogFormat("PlayerSync Awake - inRoom {0} isMine {1} photonView {2}", PhotonNetwork.inRoom, photonView.isMine, photonView.owner);
        }
        else
        {
            Debug.LogFormat("PlayerSync Awake - inRoom {0}", PhotonNetwork.inRoom);
        }
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    public void InitPlayer()
    {
        Dummy.SetActive(false);
        Real.SetActive(true);
        //Talents.SetActive(true);
    }

    bool hasUpdate;
    void Update()
    {
        if (active)
        {
            if (!GetPhotonView().isMine && PhotonNetwork.inRoom)
            {
                if (!Dummy.activeSelf)
                    Dummy.SetActive(true);
                if (rec != null && rec.enabled && !rec.Transmit)
                    rec.Transmit = true;
                if (DummyTParticles != null && Vector3.Distance(DummyHead.position, HeadPos) > 5f)
                    DummyTParticles.ParticlesOn();
                else if (DummyTParticles != null && DummyTParticles.On)
                    DummyTParticles.ParticlesOff();

                DummyHead.SetPositionAndRotation(Vector3.Lerp(DummyHead.position, HeadPos, Time.unscaledDeltaTime * InterpRate), Quaternion.Lerp(DummyHead.rotation, HeadRot, Time.unscaledDeltaTime * InterpRate));
                DummyHandLeft.SetPositionAndRotation(Vector3.Lerp(DummyHandLeft.position, LHandPos, Time.unscaledDeltaTime * InterpRate), Quaternion.Lerp(DummyHandLeft.rotation, LHandRot, Time.unscaledDeltaTime * InterpRate));
                DummyHandRight.SetPositionAndRotation(Vector3.Lerp(DummyHandRight.position, RHandPos, Time.unscaledDeltaTime * InterpRate), Quaternion.Lerp(DummyHandRight.rotation, RHandRot, Time.unscaledDeltaTime * InterpRate));         
                DummyBow.SetPositionAndRotation(Vector3.Lerp(DummyBow.position, BowPos, Time.unscaledDeltaTime * InterpRate), Quaternion.Lerp(DummyBow.rotation, BowRot, Time.unscaledDeltaTime * InterpRate));
                DummyQuiver.SetPositionAndRotation(Vector3.Lerp(DummyQuiver.position, QuiverPos, Time.unscaledDeltaTime * InterpRate), Quaternion.Lerp(DummyQuiver.rotation, QuiverRot, Time.unscaledDeltaTime * InterpRate));

                bowDummyAnim.SetFrame(Mathf.Lerp(bowDummyAnim.GetFrame(), bowPull, Time.unscaledDeltaTime * InterpRate));
                if (CheckAnims())
                {
                    lhDummyAnim.HandGrab(Mathf.Lerp(lhDummyAnim.GripValue, rhGrip, Time.unscaledDeltaTime * InterpRate));
                    rhDummyAnim.HandGrab(Mathf.Lerp(rhDummyAnim.GripValue, lhGrip, Time.unscaledDeltaTime * InterpRate));

                    lhDummyAnim.dummy = true;
                    rhDummyAnim.dummy = true;

                    //Finger Values
                    if(fingerTrack)
                    {
                        lhDummyAnim.LerpFingerPose(new KnucklesHandPose(lhK), Time.unscaledDeltaTime * 9f);
                        rhDummyAnim.LerpFingerPose(new KnucklesHandPose(rhK), Time.unscaledDeltaTime * 9f);
                    }
                }
                DummyArrowNocked.SetActive(arrowNocked);
                if (selfMute || muted)
                    src.volume = 0;
                else
                    src.volume = VolumeSettings.GetVolume(AudioType.Player);

                if (dead && !deadUpd)
                {
                    if (PlayerLife.myInstance != null)
                        PlayerLife.myInstance.PlayDeathSound(DummyHead.transform.position);
                    dummyDead.RevertEtherial();
                    dead = deadUpd;
                }
                else if (!dead && deadUpd)
                {
                    if(PlayerLife.myInstance != null)
                        PlayerLife.myInstance.PlayReviveSound(DummyHead.transform.position);
                    dummyDead.MakeEtherial();
                    dead = deadUpd;
                }

            }
            else if (!Real.activeSelf)
            {
                Real.SetActive(true);
            }
        }
        else if (Real.activeSelf || Dummy.activeSelf)
        {
            Real.SetActive(false);
            Dummy.SetActive(false);
        }
        for (int i = Others.Count - 1; i >= 0; i--)
        {
            if (Others[i] == null)
                Others.RemoveAt(i);
        }
    }
    
    void DisplayName()
    {
        PhotonPlayer player = GetPhotonView().owner;
        string fullname = player.NickName;
        if (player.CustomProperties.ContainsKey("DisplayName"))
        {
            fullname = (string)player.CustomProperties["DisplayName"];
        }
        if (player.CustomProperties.ContainsKey("fullname"))
        {
            fullname = (string)player.CustomProperties["fullname"];
        }
        if (PlayerName.text != fullname)
        {
            Debug.LogFormat("PlayerName.text changed to {0} NickName {1} CustomProperties[DisplayName] {2} CustomProperties(fullname) {3}", fullname, player.NickName, (string)player.CustomProperties["DisplayName"], (string)player.CustomProperties["fullname"]);
            PlayerName.text = fullname;
        }        
    }

    public bool isDead()
    {
        return dead;
    }

    public void ChangeDeathDisplay()
    {
        if (deadUpd)
        {
            dummyDead.MakeEtherial();
        }
        else
        {
            dummyDead.RevertEtherial();
        }
    }

    bool CheckAnims()
    {
        if ((lhRealAnim == null || rhRealAnim == null) && GetPhotonView().isMine && PhotonNetwork.inRoom)
        {
            lhRealAnim = RealHandRight.GetComponentInChildren<HandAnim>();
            rhRealAnim = RealHandLeft.GetComponentInChildren<HandAnim>();
            return (lhRealAnim != null && rhRealAnim != null);
        }
        else if (!GetPhotonView().isMine && PhotonNetwork.inRoom && (lhDummyAnim == null || rhDummyAnim == null))
        {
            lhDummyAnim = DummyHandLeft.GetComponentInChildren<HandAnim>();
            rhDummyAnim = DummyHandRight.GetComponentInChildren<HandAnim>();
            return (lhDummyAnim != null && rhDummyAnim != null);
        }
        return true;
    }

    #endregion

    #region Flags

    public void SetSelfMute(bool val)
    {
        GetPhotonView().RPC("NetworkSelfMute", PhotonTargets.OthersBuffered, val);
        selfMute = val;
    }

    [PunRPC]
    void NetworkSelfMute(bool val)
    {
        selfMute = val;
    }

    public void PersonalBubble(bool val)
    {
        GetPhotonView().RPC("NetworkBubble", PhotonTargets.OthersBuffered, val);
        bubbleOn = val;
    }

    [PunRPC]
    void NetworkBubble(bool val)
    {
        bubbleOn = val;
    }

    public bool ToggleMute()
    {
        muted = !muted;
        return muted;
    }

    #endregion

    #region Armor

    DummyArmor dummyArmor;

    public void SendArmor(string s, PhotonTargets targ=PhotonTargets.Others, PhotonPlayer singleTarg= null)
    {
        if(singleTarg == null)
            GetPhotonView().RPC("SendArmorNetwork", targ, s);
        else
            GetPhotonView().RPC("SendArmorNetwork", singleTarg, s);
    }

    public void EquipItem(string s)
    {
        GetPhotonView().RPC("EquipItemNetwork", PhotonTargets.Others, s);
    }

    public void EquipWings(int id)
    {
        GetPhotonView().RPC("EquipWingsNetwork", PhotonTargets.Others, id);
    }

    [PunRPC]
    void EquipWingsNetwork(int id)
    {
        DummyWings.EquipWings(id);
    }

    [PunRPC]
    void SendArmorNetwork(string s)
    {
        if(dummyArmor != null)
            dummyArmor.SetupArmor(s);
        List<ArmorOption> Armor = ArmorOption.ItemList(s);
        foreach(var v in Armor)
        {
            networkOutfit.Replace(v);
        }
    }

    [PunRPC]
    void EquipItemNetwork(string s)
    {
        if (dummyArmor != null)
            dummyArmor.EquipItem(s);
        networkOutfit.Replace(new ArmorOption(s));
    }

    public EffectInstance HasNetworkEffect(int eftID)
    {
        return networkOutfit.GetEffect(eftID);
    }

    #endregion

    #region Arrows

    [BitStrap.Button]
    public void TestShootArrow()
    {
        Vector3 pos = RealBow.position;
        Quaternion rot = RealBow.rotation;
        Vector3 vel = RealBow.forward * 15f;
        ShootArrowNetwork(pos, rot, vel, false, "", "");
    }

    public void ShootArrow(Vector3 pos, Quaternion rot, Vector3 vel, bool fire, string display, EffectInstance[] efts)
    {
        if(PhotonNetwork.connected)
            GetPhotonView().RPC("ShootArrowNetwork", PhotonTargets.Others, pos, rot, vel, fire, display, EffectInstance.StringList(efts));
    }

    [PunRPC]
    void ShootArrowNetwork(Vector3 pos, Quaternion rot, Vector3 vel, bool fire, string display, string elist)
    {
        DummyBow.GetComponent<BowAudio>().PlayTwang(vel.magnitude / 25f);
        GameObject arrowHold = (GameObject)Instantiate(ArrowPrefab);
        GameObject arrow = arrowHold.GetComponentInChildren<Arrow>().gameObject;
        Collider[] arrowCols = arrowHold.GetComponentsInChildren<Collider>();
        arrow.transform.SetParent(null);
        if(fire)
            arrow.GetComponentInChildren<ArrowFire>().CatchFire();
        Destroy(arrowHold);
        Collider[] DummyCols = Dummy.GetComponentsInChildren<Collider>();
        foreach (var c in arrowCols)
        {
            if(c != null)
            {
                c.enabled = true;
                foreach (var C in DummyCols)
                {
                    Physics.IgnoreCollision(c, C);
                }
            }    
        }
        if (display.Length > 1)
            arrow.GetComponentInChildren<ArrowDisplay>().ChangeDisplay(new ArmorOption(display));
        EffectInstance[] efts = EffectInstance.GetList(elist);
        arrow.GetComponent<ArrowEffects>().SetEffects(efts, false);
        arrow.transform.rotation = rot;
        arrow.transform.position = pos;
        arrow.GetComponent<Rigidbody>().isKinematic = false;
        arrow.GetComponent<Rigidbody>().velocity = vel;
        Arrow a = arrow.GetComponent<Arrow>();
        a.enabled = true;
        a.isMine = false;
        a.Init();
        a.Damage = 0;
        a.inFlight = true;
        a.Fired();
        DummyArrowNocked.SetActive(false);
        DummyArrowNocked.GetComponent<DummyArrow>().ClearEffects();
    }

    public void DestroyFiredArrow(Vector3 pos)
    {
        GetPhotonView().RPC("NetworkDestroyFiredArrow", PhotonTargets.Others, pos);
    }

    [PunRPC]
    void NetworkDestroyFiredArrow(Vector3 pos)
    {
        Arrow.DestroyNearestArrow(pos);
    }

    //Nocked Arrow Effects
    public void ArrowEffect(int[] ids)
    {
        string s = "";
        foreach (var v in ids)
        {
            s += v + ",";
        }
        GetPhotonView().RPC("NetworkArrowEffect", PhotonTargets.Others, s.Substring(0, s.Length - 1));
    }

    [PunRPC]
    void NetworkArrowEffect(string ids)
    {
        List<int> nums = new List<int>();
        foreach (var v in ids.Split(','))
        {
            int i = 0;
            int.TryParse(v, out i);
            if(!nums.Contains(i))
                nums.Add(i);
        }
        foreach (var v in nums)
        {
            DummyArrowNocked.GetComponent<DummyArrow>().DisplayEffect(v);
        }
    }

    #endregion

    #region Abilities

    public void ToggleDummyOrb(bool left, int effectID, bool on)
    {
        GetPhotonView().RPC("DummyOrbNetwork", PhotonTargets.Others, left, effectID, on);
    }

    [PunRPC]
    void DummyOrbNetwork(bool left, int eid, bool on)
    {
        if(!on)
        {
            LeftDummyOrb.SetActive(false);
            RightDummyOrb.SetActive(false);
            return;
        }

        if(!left)
        {
            LeftDummyOrb.SetActive(true);
            RightDummyOrb.SetActive(false);
            LeftDummyOrb.GetComponent<DummyOrb>().Setup(eid);
        }
        else
        {
            LeftDummyOrb.SetActive(false);
            RightDummyOrb.SetActive(true);
            RightDummyOrb.GetComponent<DummyOrb>().Setup(eid);
        }
    }

    public void ThrowOrb(int id, Vector3 pos, Vector3 vel, Vector3 avel, Quaternion rot)
    {
        GetPhotonView().RPC("ThrowOrbNetwork", PhotonTargets.Others, id, pos, vel, avel, rot);
    }

    [PunRPC]
    void ThrowOrbNetwork(int id, Vector3 pos, Vector3 vel, Vector3 avel, Quaternion rot)
    {
        OrbManager.instance.FakeOrb(id, pos, vel, avel, rot);
    }

    public void UseAbility(int id, Vector3 pos, Vector3 lookDir, float value)
    {
        GetPhotonView().RPC("UseAbilityNetworkPos", PhotonTargets.Others, id, pos, lookDir, value);
    }

    [PunRPC]
    void UseAbilityNetworkPos(int id, Vector3 pos, Vector3 lookDir, float value)
    {
        AbilityManager.instance.UseAbility(id, pos, lookDir, true, value);
    }

    #endregion

    PhotonView view;
    PhotonView GetPhotonView()
    {
        if (view == null)
            view = GetComponent<PhotonView>();
        return view;
    }

    void OnPhotonPlayerConnected(PhotonPlayer plr)
    {
        if(GetPhotonView().isMine)
        {
            SendArmor(Armory.CurrentOutfit(), PhotonTargets.Others, plr);
            int wingID = Cosmetics.instance.WingID;
            if (wingID >= 0 && wingID < Cosmetics.WingIDs.Count)
                wingID = Cosmetics.WingIDs[wingID];
            GetPhotonView().RPC("EquipWingsNetwork", plr, wingID);
        }       
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(RealHead.position);
            stream.SendNext(RealHead.rotation);

            //How did this get swapped??
            stream.SendNext(RealHandRight.position);
            stream.SendNext(RealHandRight.rotation);

            stream.SendNext(RealHandLeft.position);
            stream.SendNext(RealHandLeft.rotation);

            stream.SendNext(RealBow.position);
            stream.SendNext(RealBow.rotation);

            stream.SendNext(RealQuiver.position);
            stream.SendNext(RealQuiver.rotation);

            float lhg = 0;
            float rhg = 0;
            if(CheckAnims())
            {
                lhg = lhRealAnim.GripValue;
                rhg = rhRealAnim.GripValue;
            }
            stream.SendNext(lhg);
            stream.SendNext(rhg);

            stream.SendNext(bowRealAnim.GetFrame());
            bool hasArrow = bowAim.HasArrow();
            if (NVR_Player.isThirdPerson())
                hasArrow = NVR_Player.instance.nvbow.DummyArrow.activeInHierarchy;
            stream.SendNext(hasArrow);
            stream.SendNext(plrLife.isDead);
            stream.SendNext(active);
            stream.SendNext(PlayerHolder.Down());

            float[] rhk = new float[0];
            float[] lhk = new float[0];
            if (RHKnuckle != null)
                rhk = RHKnuckle.handPose.fingerVals();
            if (LHKnuckle != null)
                lhk = LHKnuckle.handPose.fingerVals();
            stream.SendNext(KnucklesHandControl.UsingFingers);
            stream.SendNext(rhk);
            stream.SendNext(lhk);
        }
        else
        {
            // Network player, receive data
            HeadPos = (Vector3)stream.ReceiveNext();
            HeadRot = (Quaternion)stream.ReceiveNext();

            RHandPos = (Vector3)stream.ReceiveNext();
            RHandRot = (Quaternion)stream.ReceiveNext();

            LHandPos = (Vector3)stream.ReceiveNext();
            LHandRot = (Quaternion)stream.ReceiveNext();

            BowPos = (Vector3)stream.ReceiveNext();
            BowRot = (Quaternion)stream.ReceiveNext();

            QuiverPos = (Vector3)stream.ReceiveNext();
            QuiverRot = (Quaternion)stream.ReceiveNext();

            lhGrip = (float)stream.ReceiveNext();
            rhGrip = (float)stream.ReceiveNext();
            bowPull = (float)stream.ReceiveNext();

            arrowNocked = (bool)stream.ReceiveNext();
            bool b = (bool)stream.ReceiveNext();
            if(deadUpd != b)
            {
                deadUpd = b;
                ChangeDeathDisplay();
            }
            active = (bool)stream.ReceiveNext();
            DummyChest.down = (Vector3)stream.ReceiveNext();
            fingerTrack = (bool)stream.ReceiveNext();
            rhK = (float[])stream.ReceiveNext();
            lhK = (float[])stream.ReceiveNext();
        }
    }

    void OnDestroy()
    {
        if (!mine && Others != null)
            Others.Remove(this);
    }
}
