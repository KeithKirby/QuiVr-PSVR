using UnityEngine;
using System.Collections;

public class RFX4_ShaderColorGradient : MonoBehaviour {

    public RFX4_ShaderProperties ShaderColorProperty = RFX4_ShaderProperties._TintColor;
    public Gradient Color = new Gradient();
    public float Brightness = 1;
    public float TimeMultiplier = 1;
    public bool IsLoop;
    public bool UseSharedMaterial;
    [HideInInspector] public float HUE = -1;

    [HideInInspector]
     public bool canUpdate;
    private Material mat;
    private int propertyID;
    private float startTime;
    private Color startColor;
    public bool OnStart = true;
  
    private bool isInitialized;
    private string shaderProperty;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        shaderProperty = ShaderColorProperty.ToString();
        var rend = GetComponent<Renderer>();
        if (rend==null) {
            var projector = GetComponent<Projector>();
            if (projector!=null) {
                if (!projector.material.name.EndsWith("(Instance)"))
                    projector.material = new Material(projector.material) {name = projector.material.name + " (Instance)"};
                mat = projector.material;
            }
        }
        else
        {
            if (!UseSharedMaterial) mat = rend.material;
            else mat = rend.sharedMaterial;
        }
        if (mat == null)
            return;
        if (!mat.HasProperty(shaderProperty))
            return;
        if (mat.HasProperty(shaderProperty))
            propertyID = Shader.PropertyToID(shaderProperty);
        startColor = mat.GetColor(propertyID);
        var eval = Color.Evaluate(0);
        mat.SetColor(propertyID, eval * startColor);
        isInitialized = true;
    }

    void Start()
    {
        if(OnStart)
            Play();    
    }

    [AdvancedInspector.Inspect]
    public void Play()
    {
        reverse = false;
        startTime = Time.time;
        canUpdate = true;
    }

    private void Update()
    {
        if (mat == null) return;
        var time = Time.time - startTime;
        if (canUpdate)
        {
            var eval = Color.Evaluate(time / TimeMultiplier);
            if (reverse)
                eval = Color.Evaluate(1 - (time / TimeMultiplier));
            if (HUE > -0.9f)
            {
                eval = RFX4_ColorHelper.ConvertRGBColorByHUE(eval, HUE);
                startColor = RFX4_ColorHelper.ConvertRGBColorByHUE(startColor, HUE);
            }
            mat.SetColor(propertyID, eval * startColor * Brightness);
        }
        if (time >= TimeMultiplier) {
            if (IsLoop)
            {
                startTime = Time.time;
                reverse = !reverse;
            }
            else canUpdate = false;
        }
    }

    bool reverse;
    [AdvancedInspector.Inspect]
    public void Reverse()
    {
        reverse = true;
        startTime = Time.time;
        canUpdate = true;
    }

    void OnDisable()
    {
        if (mat == null) return;
        if (UseSharedMaterial) mat.SetColor(propertyID, startColor);
        mat.SetColor(propertyID, startColor);
    }

}
