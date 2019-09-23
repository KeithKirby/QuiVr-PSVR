using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileRoll : MonoBehaviour {

    public GameObject Components;
    public float UseCD = 10f;
    public Text SeedText;
    float cd;
    string curSeed;
    public RotateTimed DieRotate;
    public AnimationCurve RotateSpeed;

    void Awake()
    {
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    void Update()
    {
        if(curSeed != TileManager.instance.CurrentSeed)
        {
            bool weekly = TileManager.instance.WeeklyMap();
            curSeed = TileManager.instance.CurrentSeed;
            SeedText.text = "Game Seed: " + curSeed.Replace("\n","");
            SeedText.color = weekly ? Color.green : Color.white;
        }
        if (cd >= 0)
            cd -= Time.deltaTime;
        DieRotate.Rotation.y = RotateSpeed.Evaluate(cd);
    }

    public void TryReroll()
    {
        if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            if(cd <= 0 && GameBase.instance.inGame && GameBase.instance.Difficulty <= 0)
            {
                cd = UseCD;
                ForceReroll();
            }
        }
    }

    [AdvancedInspector.Inspect]
    void ForceReroll()
    {
        Debug.Log("Rerolling Map...");
        DieRotate.Rotation = new Vector3(0, -1000, 0);
        TileManager.instance.CreateNew();
    }

    void OnJoinedRoom()
    {
        Components.SetActive(PhotonNetwork.isMasterClient);
    }

    void OnLeftRoom()
    {
        Components.SetActive(true);
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer plr)
    {
        if (PhotonNetwork.isMasterClient)
            Components.SetActive(true);
    }
}
