using UnityEngine;
using UnityStandardAssets.ImageEffects;
[RequireComponent(typeof(Camera))]
public class HDRAA : MonoBehaviour
{
    private Camera msaaCam;
    private GameObject msaaCamGO;

    public LayerMask LayerMask;
    public bool MSSA = true;

    public bool didRender = false;
    public Camera srcCam;

    void OnEnable()
    {
        GetComponent<VRTK.VRTK_AdaptiveQuality>().msaaLevel = 0;
        QualitySettings.antiAliasing = 0;
        //srcCam.cullingMask = 0;
        srcCam.allowHDR = true;
        Antialiasing aa = GetComponent<Antialiasing>();
        if (aa != null)
            aa.enabled = true;
    }

    void OnDisable()
    {
        GetComponent<VRTK.VRTK_AdaptiveQuality>().msaaLevel = 4;
        QualitySettings.antiAliasing = 4;
        srcCam.cullingMask = ~(1 << 18);
        srcCam.allowHDR = false;
        Antialiasing aa = GetComponent<Antialiasing>();
        if(aa != null)
            aa.enabled = false;
    }

    /*
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(QualitySettings.antiAliasing > 0)
            QualitySettings.antiAliasing = 0;
        RenderTexture rtMSSA_HDR = null;
        if (msaaCamGO == null)
        {
            msaaCamGO = new GameObject("tmpCam");
            msaaCamGO.hideFlags = HideFlags.DontSave;
            msaaCamGO.transform.parent = this.transform;
            msaaCam = msaaCamGO.AddComponent<Camera>();
        }

        msaaCamGO.SetActive(true);
        msaaCam.CopyFrom(srcCam);
        msaaCam.cullingMask = LayerMask.value;
        msaaCam.depth = -100f;

        rtMSSA_HDR = RenderTexture.GetTemporary(source.width, source.height, 24, source.format, RenderTextureReadWrite.Default, MSSA ? 4 : 1);

        msaaCam.targetTexture = rtMSSA_HDR;
        msaaCam.Render();
        msaaCam.targetTexture = null;
        msaaCamGO.SetActive(false);

        Graphics.Blit(rtMSSA_HDR, destination);
        didRender = true;

        if (rtMSSA_HDR != null)
            RenderTexture.ReleaseTemporary(rtMSSA_HDR);
    }
    */
}
