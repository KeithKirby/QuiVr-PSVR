using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OffLinkNav : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                yield return StartCoroutine(NormalSpeed(agent));
                if(agent.isActiveAndEnabled)
                    agent.CompleteOffMeshLink();
            }
            yield return null;
        }
    }

    IEnumerator NormalSpeed(NavMeshAgent agent)
    {   
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = data.startPos;
        Vector3 endPos = data.endPos;
        float dist = Vector3.Distance(startPos, endPos);
        float t = dist / agent.speed;
        float max = t;
        while (t > 0)
        {
            yield return null;
            if (agent.isActiveAndEnabled)
                agent.transform.position = Vector3.Lerp(endPos, startPos, t / max);
            t -= Time.deltaTime;
        }
    }

}
