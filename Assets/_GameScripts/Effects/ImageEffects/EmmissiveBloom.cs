using UnityEngine;
 
[RequireComponent(typeof(Camera))]
public class EmmissiveBloom : MonoBehaviour {
 
    private Camera msaaCam;
    private GameObject msaaCamGO;
 
    public LayerMask LayerMask;
    public bool MSSA = true;
 
    public bool didRender = false;
 
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        RenderTexture rtMSSA_HDR = null;
 
        var srcCam = GetComponent<Camera>();
        if (msaaCamGO == null) {
 
            msaaCamGO = new GameObject("tmpCam");
            msaaCamGO.hideFlags = HideFlags.DontSave;
            msaaCamGO.transform.parent = this.transform;
 
            msaaCam = msaaCamGO.AddComponent<Camera>();
        }
 
        msaaCamGO.SetActive(true);
        msaaCam.CopyFrom(srcCam);
        msaaCam.cullingMask = LayerMask.value;
        msaaCam.depth = -100f;
 
        rtMSSA_HDR = RenderTexture.GetTemporary(source.width, source.height, 24, source.format, RenderTextureReadWrite.Default, MSSA ? 8 : 1);
 
        msaaCam.targetTexture = rtMSSA_HDR;
        msaaCam.Render();
        msaaCam.targetTexture = null;
        msaaCamGO.SetActive(false);
 
        Graphics.Blit(rtMSSA_HDR, destination);
        didRender = true;
 
        if (rtMSSA_HDR != null)
            RenderTexture.ReleaseTemporary(rtMSSA_HDR);
    }
}