using UnityEngine;
using System.Collections;

public class PortalSpawner : MonoBehaviour {

    public GameObject portalCamera;
    public GameObject portalViewer;

    public void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Ground")
        {
            portalCamera.transform.position = col.contacts[0].point;
            Vector3 pos = portalCamera.transform.position + GetComponent<Rigidbody>().velocity;
            portalCamera.transform.LookAt(new Vector3(pos.x, portalCamera.transform.position.y, pos.z));
            SteamVR_Camera playerHead = FindObjectOfType<SteamVR_Camera>();
            if(playerHead != null)
            {
                portalViewer.transform.position = playerHead.transform.position + new Vector3(playerHead.transform.forward.x, 0, playerHead.transform.forward.z)*4f;
                portalViewer.transform.LookAt(playerHead.transform);
            }
            portalViewer.SetActive(true);
            portalCamera.SetActive(true);
            Destroy(gameObject);
        }
    }
}
