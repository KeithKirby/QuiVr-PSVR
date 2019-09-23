using UnityEngine;

public class VRUIComplete : MonoBehaviour {
    public GameObject target;
    public string message = "";
    public bool disableColliderAfterUse = false;

	public void Complete () {
        target.SendMessage(message, SendMessageOptions.DontRequireReceiver);

        if (disableColliderAfterUse && GetComponent<BoxCollider>())
            GetComponent<BoxCollider>().enabled = false;
    }
}
