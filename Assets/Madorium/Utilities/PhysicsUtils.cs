using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class PhysicsUtils
{
    static RaycastHit[] _resultsBuffer = new RaycastHit[1];

    static public bool RaycastNonAlloc(Vector3 start, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask = ~0)
    {
        int numHits = Physics.RaycastNonAlloc(start, direction, _resultsBuffer, maxDistance, layerMask);
        float closestDist = float.MaxValue;
        int closest = 0;
        if(numHits > 0)
        {
            for (int i = 0; i < numHits; ++i)
            {
                var res = _resultsBuffer[i];
                if(res.distance < closestDist)
                {
                    closest = i;
                    closestDist = res.distance;
                }
            }
            hit = _resultsBuffer[closest];
            return true;
        }

        hit = _resultsBuffer[0];
        return false;
    }	
}
