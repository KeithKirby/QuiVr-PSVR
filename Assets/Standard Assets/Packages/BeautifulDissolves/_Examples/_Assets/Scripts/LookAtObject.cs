using UnityEngine;
using System.Collections;

namespace BeautifulDissolves {
	public class LookAtObject : MonoBehaviour {

		public Transform target;
        public float lerpSpeed;
		private Transform m_Transform;

		void Awake()
		{
			m_Transform = GetComponent<Transform>();
		}

		void Update ()
		{
            if(target != null)
            {
                if (lerpSpeed <= 0)
                    m_Transform.LookAt(target);
                else
                {
                    Quaternion wantRot = Quaternion.LookRotation(target.position-transform.position, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, wantRot, Time.deltaTime * lerpSpeed);
                }

            }
        }
	}
}
