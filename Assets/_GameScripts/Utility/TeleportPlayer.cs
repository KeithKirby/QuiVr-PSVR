using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeleportPlayer : MonoBehaviour {

    TeleportNode startNode;

    public GameObject PlayerArea;

    public TeleportNode BlacksmithLocation;
    public TeleportNode WallTop;
    public AudioSource aud;

    public ChestFollow chestObj;

	List<TeleportNode> Nodes;

    public GameObject head;

    public AudioClip[] teleportSounds;

    TeleportNode CurrentNode;

    public void SetAllActive()
    {
        foreach(var v in Nodes)
        {
            v.Show();
        }
    }

    public void DisableAll()
    {
        foreach (var v in Nodes)
        {
            v.Hide();
        }
    }

    public void ReleaseMine()
    {
        if (PhotonNetwork.inRoom && CurrentNode != null)
            CurrentNode.ReleaseSpot(PhotonNetwork.player.ID);
    }

    public void Start()
    {
		Nodes = new List<TeleportNode> ();
		var nds = FindObjectsOfType<TeleportNode>();
        foreach(var v in nds)
        {
			Nodes.Add (v);
            v.Init(this);
            v.GetComponent<LookAt>().target = head.transform;
            if (v.startNode)
                startNode = v;
        }
    }

    public SteamVR_PlayArea area;

    float ForwardOffset()
    {
        float o = 0;
        Vector3[] verts = area.vertices;
        foreach(var v in verts)
        {
            o += Mathf.Abs(v.z);
        }
        o /= 4f;
        o *= 0.2f;
        return o;
    }

	Transform currentPos;

	void Update()
	{
		if (currentPos != null) {
			PlayerArea.transform.position = currentPos.position - (currentPos.TransformDirection(Vector3.forward)*ForwardOffset());
			PlayerArea.transform.rotation = currentPos.rotation;
		}
	}

	public void Teleport(TeleportNode node)
    {
        if (!Nodes.Contains(node))
            Nodes.Add(node);
        if (!waiting)
            StartCoroutine("AttemptTeleport", node);
    }

    bool recievedResponse;
    bool waiting;
    int spotID;
    IEnumerator AttemptTeleport(TeleportNode node)
    {
        bool needResponse = false;
        if(PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
        {
            needResponse = true;
            waiting = true;
            recievedResponse = false;
            node.RequestSpotID();
            while(!recievedResponse)
            {
                yield return true;
            }
            Debug.Log("Got Spot ID: " + spotID);
            if(spotID < 0)
            {
                StopCoroutine("AttemptTeleport");
            }
        }
        if (PhotonNetwork.inRoom && CurrentNode != null)
            CurrentNode.ReleaseSpot(PhotonNetwork.player.ID);
        Transform obj;
        if (needResponse)
            obj = node.GetLocation(spotID);
        else
            obj = node.GetLocation();
        CurrentNode = node;
        currentPos = obj;
        PlayerArea.transform.position = obj.position - (obj.TransformDirection(Vector3.forward) * ForwardOffset());
        PlayerArea.transform.rotation = obj.rotation;
        if (chestObj != null)
            chestObj.ResetChestWeightTarget();
        foreach (var v in Nodes)
        {
            if (!v.isFull())
                v.Show();
            if (v == node)
            {
                v.Hide();
            }
        }
        aud.clip = teleportSounds[Random.Range(0, teleportSounds.Length)];
        aud.Play();
        StopCoroutine("AttemptTeleport");
    }

    public void RecieveResponse(int id)
    {
        Debug.Log("Got Response: " + id);
        if(id == -1)
        {
            StopCoroutine("AttemptTeleport");
            waiting = false;
        }
        else
        {
            spotID = id;
        }
        recievedResponse = true;
        waiting = false;
    }
}
