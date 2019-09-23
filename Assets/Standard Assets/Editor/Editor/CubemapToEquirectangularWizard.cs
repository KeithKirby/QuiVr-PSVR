// Wizard to convert a cubemap to an equirectangular cubemap.
// Put this into an /Editor folder
// Run it from Tools > Cubemap to Equirectangular Map

using UnityEditor;
using UnityEngine;

using System.IO;

class CubemapToEquirectangularWizard : ScriptableWizard
{

    public Cubemap cubeMap = null;
    public int equirectangularWidth = 2048;
    public int equirectangularHeight = 1024;

    private Material cubemapToEquirectangularMaterial;
    private Shader cubemapToEquirectangularShader;

    [MenuItem("Tools/Cubemap to Equirectangular Map")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CubemapToEquirectangularWizard>("Cubemap to Equirectangular Map", "Convert");
    }

    void OnWizardCreate()
    {

        bool goodToGo = true;

        cubemapToEquirectangularShader = Shader.Find("Custom/cubemapToEquirectangular");
        if (cubemapToEquirectangularShader == null)
        {
            Debug.LogWarning("Couldn't find the shader \"Custom/cubemapToEquirectangular\", do you have it in your project?\nYou can get it here; https://gist.github.com/Farfarer/5664694#file-cubemaptoequirectangular-shader");
            goodToGo = false;
        }
        else
        {
            cubemapToEquirectangularMaterial = new Material(cubemapToEquirectangularShader);
        }

        if (cubeMap == null)
        {
            Debug.LogWarning("You must specify a cubemap.");
            goodToGo = false;
        }
        else if (equirectangularWidth < 1)
        {
            Debug.LogWarning("Width must be greater than 0.");
            goodToGo = false;
        }
        else if (equirectangularHeight < 1)
        {
            Debug.LogWarning("Height must be greater than 0.");
            goodToGo = false;
        }

        if (goodToGo)
        {
            // Go to gamma space.
            ColorSpace originalColorSpace = PlayerSettings.colorSpace;
            PlayerSettings.colorSpace = ColorSpace.Gamma;

            // Do the conversion.
            RenderTexture rtex_equi = new RenderTexture(equirectangularWidth, equirectangularHeight, 24);
            Graphics.Blit(cubeMap, rtex_equi, cubemapToEquirectangularMaterial);

            Texture2D equiMap = new Texture2D(equirectangularWidth, equirectangularHeight, TextureFormat.ARGB32, false);
            //equiMap.SetPixels(rtex_equiPixels);
            equiMap.ReadPixels(new Rect(0, 0, equirectangularWidth, equirectangularHeight), 0, 0, false);
            equiMap.Apply();

            byte[] bytes = equiMap.EncodeToPNG();
            DestroyImmediate(equiMap);

            string assetPath = AssetDatabase.GetAssetPath(cubeMap);
            string assetDir = System.IO.Path.GetDirectoryName(assetPath);
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath) + "_equirectangular.png";
            string newAsset = System.IO.Path.Combine(assetDir, assetName);
            File.WriteAllBytes(newAsset, bytes);

            // Import the new texture.
            AssetDatabase.ImportAsset(newAsset);

            Debug.Log("Equirectangular map saved to " + newAsset);

            // Go to whatever the color space was before.
            PlayerSettings.colorSpace = originalColorSpace;
        }
    }

    void OnWizardUpdate()
    {
        helpString = "Converts a cubemap into an equirectangular map.";
    }
}