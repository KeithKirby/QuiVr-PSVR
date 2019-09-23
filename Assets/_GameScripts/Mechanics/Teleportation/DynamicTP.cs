using UnityEngine;
using System.Collections;

public class DynamicTP : MonoBehaviour {

    Teleporter t;
    PlayerPositions p;
    SpriteRenderer[] sprites;

    void Awake()
    {
        t = GetComponent<Teleporter>();
        p = GetComponent<PlayerPositions>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public void Setup(bool useLocal,  Vector3[] positions, Quaternion[] rotations, float SpriteScale = 1f)
    {
        //Set Scale
        if(SpriteScale != 1)
            SetScale(SpriteScale);
        //Set Player Positions
        int pNum = Mathf.Clamp(positions.Length, 0, p.positions.Length);
        for (int i=0; i<pNum; i++)
        {
            if(useLocal)
            {
                p.positions[i].pos.localPosition = positions[i];
                p.positions[i].pos.localRotation = rotations[i];
            }  
            else
            {
                p.positions[i].pos.position = positions[i];
                p.positions[i].pos.rotation = rotations[i];
            }
                
        }
        p.positions = p.positions.SubArray(0, pNum);
        t.EnableTeleport();
    }

    void SetScale(float scale)
    {
        foreach(var v in sprites)
        {
            v.transform.localScale *= scale;
        }
    }

    void OnJoinedRoom()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (TelePlayer.instance != null && TelePlayer.instance.currentNode == t && !TelePlayer.instance.s_tryingTeleport)
            TelePlayer.instance.TeleportClosest(t);
    }
}
