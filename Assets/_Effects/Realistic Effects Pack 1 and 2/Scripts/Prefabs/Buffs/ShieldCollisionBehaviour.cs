using UnityEngine;
using System.Collections;

public class ShieldCollisionBehaviour : MonoBehaviour
{
      public GameObject EffectOnHit;
      public GameObject ExplosionOnHit;
      public float ScaleWave = 0.89f;
      public Vector3 AngleFix;
      public int currentQueue = 3001;
      public bool WorldPosition;

    void OnCollisionEnter(Collision e)
    {
        ShieldCollisionEnter(e);
    }

    public void ShieldCollisionEnter(Collision e)
    {
        if (e.collider.transform!=null)
        {
            var effectInstance = Instantiate(EffectOnHit) as GameObject;
            Destroy(effectInstance, 5f);
            var tr = effectInstance.transform;
            effectInstance.SetActive(true);
            tr.parent = transform;
            tr.localPosition = Vector3.zero;
            if (ScaleWave == 1)
                tr.localScale = Vector3.one;
            else
                tr.localScale = transform.localScale * ScaleWave;
            if (!WorldPosition)
            {
                tr.LookAt(e.contacts[0].point);
                tr.Rotate(AngleFix);
            }
            else
            {
                tr.rotation = transform.rotation;
                tr.position = e.contacts[0].point;
            }
            Renderer r = effectInstance.GetComponent<Renderer>();
            if (r != null)
                r.GetComponent<Renderer>().material.renderQueue = currentQueue - 1000;

            if (currentQueue > 4000) currentQueue = 3001;
            else ++currentQueue;

            if (ExplosionOnHit!=null) {
                var inst2 = Instantiate(ExplosionOnHit, e.contacts[0].point, new Quaternion()) as GameObject;
                inst2.transform.parent = transform;
            }
        }
    }

    public void OnArrow(ArrowCollision col)
    {
        var effectInstance = Instantiate(EffectOnHit) as GameObject;
        Destroy(effectInstance, 5f);
        var tr = effectInstance.transform;
        effectInstance.SetActive(true);
        tr.parent = transform;
        tr.localPosition = Vector3.zero;
        if (ScaleWave == 1)
            tr.localScale = Vector3.one;
        else
            tr.localScale = transform.localScale * ScaleWave;
        if(!WorldPosition)
        {
            tr.LookAt(col.impactPos);
            tr.Rotate(AngleFix);
        }
        else
        {
            tr.rotation = transform.rotation;
            tr.position = col.impactPos;
        }
        Renderer r = effectInstance.GetComponent<Renderer>();
        if (r != null)
            r.GetComponent<Renderer>().material.renderQueue = currentQueue - 1000;

        if (currentQueue > 4000) currentQueue = 3001;
        else ++currentQueue;
    }



  //  void OnTriggerEnter(Collider collider)
  //  {
  //    pos = transform.position;
  //    Vector3 hitPoint = Vector3.zero;
  //    if (!IsDefaultCollisionPoint)
  //    {
  //      RaycastHit hit;
  //      Physics.Raycast(transform.position, (collider.transform.position - pos).normalized, out hit);
  //      hitPoint = hit.point;
  //    }
  //    if (effect!=null) {
  //      var part = effect.GetComponent<ParticleSystem>();
  //      if (part!=null) {
  //        part.startSize = transform.lossyScale.x;
  //      }
  //      else {
  //        effect.transform.localScale = transform.lossyScale;
  //      }
  //      var inst1 = Instantiate(effect) as GameObject;
  //      inst1.transform.parent = gameObject.transform;
  //      inst1.transform.localPosition = transform.localPosition + FixInctancePosition;
  //      if (IsDefaultCollisionPoint) inst1.transform.localRotation = new Quaternion();
  //      else
  //        inst1.transform.LookAt(hitPoint);
  //      inst1.transform.Rotate(FixInctanceAngle);
  //      inst1.transform.localScale = transform.localScale * FixInctanceScalePercent / 100f;
  //    }
  //    if (explosion != null)
  //      {
  //        var inst2 = Instantiate(explosion, hitPoint, new Quaternion()) as GameObject;
  //          inst2.transform.parent = transform;
  //      }
  //  }
}
