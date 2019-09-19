using UnityEngine;
using System.Collections;

public class RFX4_ShaderFloatCurve : MonoBehaviour {

    public RFX4_ShaderProperties ShaderFloatProperty = RFX4_ShaderProperties._Cutoff;
    public string ShaderPropOverride;
    public AnimationCurve FloatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float GraphTimeMultiplier = 1, GraphIntensityMultiplier = 1;
    public bool IsLoop;
    public bool UseSharedMaterial;

    private bool canUpdate;
    private float startTime;
    private Material mat;
    private float startFloat;
    private int propertyID;
    private string shaderProperty;
    private bool isInitialized;
    public bool OnStart = true;

    private void Awake()
    {
        //mat = GetComponent<Renderer>().material;
        var rend = GetComponent<Renderer>();
        if (rend == null)
        {
            var projector = GetComponent<Projector>();
            if (projector != null)
            {
                if (!UseSharedMaterial)
                {
                    if (!projector.material.name.EndsWith("(Instance)"))
                        projector.material = new Material(projector.material) { name = projector.material.name + " (Instance)" };
                    mat = projector.material;
                }
                else
                {
                    mat = projector.material;
                }
            }
        }
        else
        {
            if (!UseSharedMaterial) mat = rend.material;
            else mat = rend.sharedMaterial;
        }
      

        shaderProperty = ShaderFloatProperty.ToString();
        if (ShaderPropOverride.Length > 0)
            shaderProperty = ShaderPropOverride;
        if (mat.HasProperty(shaderProperty)) propertyID = Shader.PropertyToID(shaderProperty);
        if (ShaderFloatProperty == RFX4_ShaderProperties._Color || ShaderFloatProperty == RFX4_ShaderProperties._TintColor)
            startFloat = mat.GetColor(shaderProperty).a;
        else
            startFloat = mat.GetFloat(propertyID);
        var eval = FloatCurve.Evaluate(0) * GraphIntensityMultiplier;
        if (ShaderFloatProperty == RFX4_ShaderProperties._Color || ShaderFloatProperty == RFX4_ShaderProperties._TintColor)
        {
            Color c = mat.GetColor(shaderProperty);
            c.a = eval;
            mat.SetColor(shaderProperty, c);
        }
        else
            mat.SetFloat(propertyID, eval);
        isInitialized = true;
    }

    private void OnEnable()
    {
        if (OnStart)
            Play();
    }

    public void Play()
    {
        startTime = Time.time;
        reverse = false;
        canUpdate = true;
        if (isInitialized)
        {
            var eval = FloatCurve.Evaluate(0) * GraphIntensityMultiplier;
            mat.SetFloat(propertyID, eval);
        }
    }

    private void Update()
    {
        var time = Time.time - startTime;
        if (canUpdate)
        {
            var eval = FloatCurve.Evaluate(time / GraphTimeMultiplier) * GraphIntensityMultiplier;
            if(reverse)
                eval = FloatCurve.Evaluate(1-(time / GraphTimeMultiplier)) * GraphIntensityMultiplier;
            if (ShaderFloatProperty == RFX4_ShaderProperties._Color ||ShaderFloatProperty == RFX4_ShaderProperties._TintColor)
            {
                Color c = mat.GetColor(shaderProperty);
                c.a = eval;
                mat.SetColor(shaderProperty, c);
            }                
            else
                mat.SetFloat(propertyID, eval);
        }
        if (time >= GraphTimeMultiplier)
        {
            if (IsLoop) startTime = Time.time;
            else canUpdate = false;
        }
    }

    bool reverse;
    public void Reverse()
    {
        reverse = true;
        startTime = Time.time;
        canUpdate = true;
    }
   
    void OnDisable()
    {
        if(UseSharedMaterial) mat.SetFloat(propertyID, startFloat);
    }

    void OnDestroy()
    {
        if (!UseSharedMaterial)
        {
            if (mat != null)
                DestroyImmediate(mat);
            mat = null;
        }
    }
}
