using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentDebugger : MonoBehaviour {

    NavMeshAgent agent;

    public bool OnNavMesh;
    public bool HasPath;
    public bool PathPending;
    public bool OnOffMeshLink;
    public bool Stopped;
    public bool pathStale;
    Vector3[] Corners;
    public Vector3 Destination;
    NavMeshPathStatus PathStatus;
    public float DistanceToDest;

	void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(agent != null)
        {
            OnNavMesh = agent.isOnNavMesh;
            HasPath = agent.hasPath;
            PathPending = agent.pathPending;
            OnOffMeshLink = agent.isOnOffMeshLink;
            if(HasPath)
            {
                pathStale = agent.isPathStale;                
                Corners = agent.path.corners;
                for(int i=0; i<Corners.Length; i++)
                {
                    if (i < Corners.Length - 1)
                        Debug.DrawLine(Corners[i], Corners[i + 1], Color.red);
                }
                Destination = agent.destination;
                Stopped = agent.isStopped;
                PathStatus = agent.pathStatus;
                DistanceToDest = agent.remainingDistance;
            }
            
        }
    }

    [AdvancedInspector.Inspect]
    public void WarpToDest()
    {
        if(agent != null && agent.isOnNavMesh && agent.hasPath)
        {
            agent.Warp(agent.destination);
        }
    }
}
