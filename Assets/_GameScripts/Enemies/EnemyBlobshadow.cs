using UnityEngine;
using System.Collections;

public class EnemyBlobshadow : MonoBehaviour {

    public GameObject BlobShadow;

    private void Start()
    {
        if (QualitySettings.shadows == UnityEngine.ShadowQuality.Disable)
            BlobShadow.SetActive(true);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "DarkArea" && BlobShadow != null && !BlobShadow.activeSelf)
        {
            BlobShadow.SetActive(true);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "DarkArea" && BlobShadow != null && BlobShadow.activeSelf)
        {
            BlobShadow.SetActive(false);
        }
    }
}
