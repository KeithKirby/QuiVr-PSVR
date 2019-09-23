using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeautifulDissolves;
public class HandFade : MonoBehaviour {

    Transform CameraTarget;
    public List<Renderer> renderers;
    Dissolve[] Dissolves;
    ParticleSystem[] psystems;


    public float distThresh = 0.23f;
    public float offset = 0.1f;
    float dist;

    public bool dummy;
    public bool forceFade;
    public bool selfPos;
    PlayerSync ps;

    public float UpdateRate = 0.1f;

	void Awake()
    {
        renderers = new List<Renderer>();
        psystems = new ParticleSystem[] { };
        CheckMaterial();
        if(dummy)
        {
            ps = GetComponentInParent<PlayerSync>();
        }
        InvokeRepeating("UpdateMat", 0.1f, UpdateRate);
    }
	
	void UpdateMat()
    {
        opacity = 1;
        if (CameraTarget == null && PlayerHead.instance != null)
            CameraTarget = PlayerHead.instance.transform;
        CheckMaterial();
        if ((renderers.Count < 1 && Dissolves.Length < 1) || CameraTarget == null)
            return;
        bool spect = false;
        if (SpectatorSync.myInstance != null && SpectatorSync.myInstance.active)
            spect = SpectatorSync.myInstance.active;
        float dt = distThresh;
        float ofs = offset;
        bool bubbleOn = false;
        if (ps != null)
            bubbleOn = ps.bubbleOn;
        if ((Settings.GetBool("PersonalBubble") || bubbleOn) && dummy)
        {
            dt = 1.6f;
            ofs = 1.4f;
        }
        foreach(var v in psystems)
        {
            dist = Vector3.Distance(CameraTarget.position, v.transform.position);
            if (dummy)
                dist = Vector2.Distance(new Vector2(CameraTarget.position.x, CameraTarget.position.z), new Vector2(transform.position.x, v.transform.position.z));
            else if (selfPos)
                dist = Vector3.Distance(CameraTarget.position, transform.position);
            var em = v.emission;
            em.enabled = dist > dt;
        }
        foreach(var d in Dissolves)
        {
            dist = Vector3.Distance(CameraTarget.position, d.transform.position);
            if (dummy)
                dist = Vector2.Distance(new Vector2(CameraTarget.position.x, CameraTarget.position.z), new Vector2(transform.position.x, d.transform.position.z));
            else if (selfPos)
                dist = Vector3.Distance(CameraTarget.position, transform.position);
            float distPerc = Mathf.Clamp((dist - ofs) / (dt / 2), 0f, 1f);
            opacity = distPerc;
            if (distPerc < 0.1)
                distPerc = -1;
            d.SetDissolveAmount(1-distPerc);
        }
        foreach(var v in renderers)
        {
            if(v != null)
            {
                dist = Vector3.Distance(CameraTarget.position, v.transform.position);
                if (dummy)
                    dist = Vector2.Distance(new Vector2(CameraTarget.position.x, CameraTarget.position.z), new Vector2(transform.position.x, v.transform.position.z));
                else if(selfPos)
                    dist = Vector3.Distance(CameraTarget.position, transform.position);
               
                Material[] mats = v.materials;
                foreach (var m in mats)
                {
                    if(m.HasProperty("_Color"))
                    {
                        Color c = m.GetColor("_Color");
                        c.a = 1;
                        if (dist < dt && !spect)
                        {
                            if (m.HasProperty("_Mode"))
                            {
                                m.SetFloat("_Mode", 2);
                                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                m.SetInt("_ZWrite", 0);
                                m.DisableKeyword("_ALPHATEST_ON");
                                m.EnableKeyword("_ALPHABLEND_ON");
                                m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                                m.renderQueue = 3000;
                            }
                            float distPerc = Mathf.Clamp((dist - ofs) / (dt / 2), 0f, 1f);
                            opacity = distPerc;
                            c = new Color(c.r, c.g, c.b, distPerc);
                        }
                        else if (m.HasProperty("_Mode"))
                        {
                            m.SetFloat("_Mode", 0);
                            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                            m.SetInt("_ZWrite", 1);
                            m.DisableKeyword("_ALPHATEST_ON");
                            m.DisableKeyword("_ALPHABLEND_ON");
                            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            m.renderQueue = 3000;
                        }
                        m.SetColor("_Color", c);
                    }
                }
            }
        }
	}

    float opacity;
    public float GetOpacity()
    {
        return opacity;
    }

    void CheckMaterial()
    {
        if((Dissolves == null || Dissolves.Length < 1) && !forceFade)
        {
            SetupLists();
            return;
        }
        else if(forceFade && (renderers == null || renderers.Count < 1))
        {
            SetupLists();
            return;
        }
        bool missing = false;
        foreach(var v in renderers)
        {
            if (v == null) 
                missing = true;
        }
        foreach (var v in Dissolves)
        {
            if (v == null)
                missing = true;
        }
        if (missing)
        {
            SetupLists();
        }
    }

    void SetupLists()
    {
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        Dissolves = GetComponentsInChildren<Dissolve>();
        if (forceFade)
            Dissolves = new Dissolve[] { };
        psystems = GetComponentsInChildren<ParticleSystem>();
        renderers = new List<Renderer>();
        foreach(var r in rs)
        {
            bool inDissolves = false;
            foreach(var d in Dissolves)
            {
                if (d.gameObject == r.gameObject)
                    inDissolves = true;
            }
            if (!inDissolves)
                renderers.Add(r);
        }
    }
}
