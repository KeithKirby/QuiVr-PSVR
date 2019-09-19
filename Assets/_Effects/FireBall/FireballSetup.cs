using UnityEngine;
using System.Collections;

public class FireballSetup : MonoBehaviour {

    public float speedOverride;

    public static void SetupFireball(GameObject fb, Vector3 target, float positionVariance = 0, float speedVariance = 0)
    {
        //ValveCamera plr = FindObjectOfType<ValveCamera>();
        Vector3 cPos = target;//Vector3.zero;
       // if (plr == null)
            //return;
       // cPos = plr.transform.position;
        if(fb.GetComponent<ProjectileLob>() != null)
        {
            fb.GetComponent<ProjectileLob>().SetTarget(cPos);
            fb.GetComponent<Rigidbody>().useGravity = true;
            fb.GetComponent<ProjectileLob>().Shoot();
        }
        else if(fb.GetComponent<Fireball>() != null)
        {
            Vector3 modPos = new Vector3(cPos.x + Random.Range(-1*positionVariance, positionVariance), 
                                         cPos.y + Random.Range(-1 * positionVariance, positionVariance), 
                                         cPos.z + Random.Range(-1 * positionVariance, positionVariance));
            Fireball mfb = fb.GetComponent<Fireball>();
            float speed = mfb.Speed + Random.Range(-1*speedVariance, speedVariance);
            mfb.Speed = speed;
            mfb.Init(modPos);
        }    
    }

    public void SetupFireballPlr(GameObject obj)
    {
        if (PlayerHead.instance != null)
            SetupFireball(obj, PlayerHead.instance.transform.position);
        if(speedOverride > 0)
        {
            Fireball fb = obj.GetComponent<Fireball>();
            fb.Speed = speedOverride;
        }
    }
}
