using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class ArrowSpawner : MonoBehaviour {

    public float Cooldown = 30f;
    public int[] EffectIDs;
    public GameObject ArrowDisplay;

    public AudioClip SpawnClip;
    public AudioClip UseClip;
    public ParticleSystem SpawnBurst;

    bool available;
    float cd;

    void Start()
    {
        if (GameBase.instance != null)
            GameBase.instance.OnStartGame.AddListener(ClearCD);
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    #region Usage

    public void TryUse()
    {
        if (!available)
        {
            ArrowDisplay.SetActive(false);
            return;
        }
        Use();
        ResetCooldown(true);
    }

    [PunRPC]
    void ResetCooldown(bool SendToOthers)
    {
        if (available)
            return;
        if (PhotonNetwork.inRoom && SendToOthers)
            GetComponent<PhotonView>().RPC("ResetCooldown", PhotonTargets.Others, false);
        cd = Cooldown;
        if (UseClip != null)
            VRAudio.PlayClipAtPoint(UseClip, transform.position, 0.5f, 1f, 1f, 1);
    }

    void Use()
    {
        available = false;
        ArrowDisplay.SetActive(false);
        if (Quiver.instance != null)
            Quiver.instance.TryGiveArrow(EffectIDs);
        ArrowDisplay.GetComponent<VRTK_InteractableObject>().SaveCurrentState();
    }

    [BitStrap.Button]
    public void ClearCD()
    {
        cd = 0;
    }

    void Update()
    {
        cd -= Time.deltaTime;
        if (cd <= 0 && !available)
        {
            available = true;
            ArrowDisplay.SetActive(true);
            if (SpawnClip != null)
                VRAudio.PlayClipAtPoint(SpawnClip, transform.position, 0.5f, 1f, 1f, 1);
            if(SpawnBurst != null)
                SpawnBurst.Play();
        }   
    }

    #endregion

    #region Networking

    void OnPhotonPlayerConnected(PhotonPlayer plr)
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            GetComponent<PhotonView>().RPC("SetCD", plr, cd);
    }

    [PunRPC]
    void SetCD(float val)
    {
        cd = val;
        if(cd > 0)
            ArrowDisplay.SetActive(false);
    }

    #endregion

}
