using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Vive.Plugin.SR
{
    public class ReconstructedAssetImporter : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            // assetPath;
            // assetImporter;
            if (!assetPath.Contains("/Recons3DAsset/"))
                return;

            ModelImporter importer = assetImporter as ModelImporter;
            importer.meshCompression = ModelImporterMeshCompression.Off;
            importer.optimizeMesh = false;
            importer.importBlendShapes = false;
            importer.isReadable = false;

            if (assetPath.Contains("/VertexColor/"))        // not used
                importer.importMaterials = false;
            else
                importer.importMaterials = true;

            importer.importNormals = ModelImporterNormals.None;
            importer.importTangents = ModelImporterTangents.None;

            if (assetPath.Contains("_cld.obj"))
                importer.importMaterials = false;
            else
                importer.materialSearch = ModelImporterMaterialSearch.Local;
        }

        void OnPreprocessTexture()
        {
            // assetPath;
            // assetImporter;
            if (!assetPath.Contains("/Recons3DAsset/"))
                return;

            TextureImporter importer = assetImporter as TextureImporter;
            importer.mipmapEnabled = false;
        }

        //Material OnAssignMaterialModel(Material mtr, Renderer rnd)
        //{
        //    string mtrPath = "Assets/Recons3DAsset/" + mtr.name + ".mat";
        //    // if exists
        //    Material ret = AssetDatabase.LoadAssetAtPath<Material>(mtrPath);
        //    if (ret != null)
        //        return ret;

        //    // else create new
        //    mtr.shader = Shader.Find("Unlit/Texture");
        //    AssetDatabase.CreateAsset(mtr, mtrPath);
        //    return mtr;
        //}

        class ColliderProperty
        {
            public bool IsConvex = false;
            public bool IsBoundingRect = false;
            public bool IsFrontFace = false;
            public bool IsBackFace = false;
            public bool IsHorizontal = false;
            public bool IsVertical = false;
        }


        void OnPostprocessModel(GameObject curGO)
        {
            // handles collider
            //if (!assetPath.Contains("/Recons3DAsset/") || !assetPath.Contains("_cld.obj"))
            if (!assetPath.Contains("/Recons3DAsset/"))
                return;

            // collect colliders
            bool hasCollider = false;
            MeshRenderer[] rnds = curGO.GetComponentsInChildren<MeshRenderer>(true);
            List<ColliderProperty> propertyList = new List<ColliderProperty>();
            int len = rnds.Length;
            for (int i = 0; i < len; ++i)
            {
                ColliderProperty property = new ColliderProperty();     
                if (rnds[i].name.Contains("PlaneConvexCollider"))
                    hasCollider = property.IsConvex = true;
                else if (rnds[i].name.Contains("PlaneBBCollider"))
                    hasCollider = property.IsBoundingRect = true;

                if (rnds[i].name.Contains("Front"))
                    hasCollider = property.IsFrontFace = true;
                else if (rnds[i].name.Contains("Back"))
                    hasCollider = property.IsBackFace = true;

                if (rnds[i].name.Contains("Horizontal"))
                    hasCollider = property.IsHorizontal = true;
                else if (rnds[i].name.Contains("Vertical"))
                    hasCollider = property.IsVertical = true;

                propertyList.Add(property);
                rnds[i].sharedMaterial.shader = Shader.Find("Unlit/Texture");
            }

            if (hasCollider == false) return;

            GameObject convexCldGroup = new GameObject("PlaneConvexCollider");
            {                
                convexCldGroup.transform.SetParent(curGO.transform);
                GameObject HorizontalGroup = new GameObject("Horizontal");
                {
                    HorizontalGroup.transform.SetParent(convexCldGroup.transform);
                    GameObject FrontGroup = new GameObject("FrontFace");
                    GameObject BackGroup = new GameObject("BackFace");
                    FrontGroup.transform.SetParent(HorizontalGroup.transform);
                    BackGroup.transform.SetParent(HorizontalGroup.transform);
                }
                GameObject VerticalGroup = new GameObject("Vertical");
                {
                    VerticalGroup.transform.SetParent(convexCldGroup.transform);
                    GameObject FrontGroup = new GameObject("FrontFace");
                    GameObject BackGroup = new GameObject("BackFace");
                    FrontGroup.transform.SetParent(VerticalGroup.transform);
                    BackGroup.transform.SetParent(VerticalGroup.transform);
                }
                {
                    GameObject FrontGroup = new GameObject("FrontFace");
                    GameObject BackGroup = new GameObject("BackFace");
                    FrontGroup.transform.SetParent(convexCldGroup.transform);
                    BackGroup.transform.SetParent(convexCldGroup.transform);
                }
            }

            GameObject bbCldGroup = new GameObject("PlaneBoundingRectCollider");
            {
                bbCldGroup.transform.SetParent(curGO.transform);
                GameObject HorizontalGroup = new GameObject("Horizontal");
                {
                    HorizontalGroup.transform.SetParent(bbCldGroup.transform);
                    GameObject FrontGroup = new GameObject("FrontFace");
                    GameObject BackGroup = new GameObject("BackFace");
                    FrontGroup.transform.SetParent(HorizontalGroup.transform);
                    BackGroup.transform.SetParent(HorizontalGroup.transform);
                }
                GameObject VerticalGroup = new GameObject("Vertical");
                {
                    VerticalGroup.transform.SetParent(bbCldGroup.transform);
                    GameObject FrontGroup = new GameObject("FrontFace");
                    GameObject BackGroup = new GameObject("BackFace");
                    FrontGroup.transform.SetParent(VerticalGroup.transform);
                    BackGroup.transform.SetParent(VerticalGroup.transform);
                }
                {
                    GameObject FrontGroup = new GameObject("FrontFace");
                    GameObject BackGroup = new GameObject("BackFace");
                    FrontGroup.transform.SetParent(bbCldGroup.transform);
                    BackGroup.transform.SetParent(bbCldGroup.transform);
                }
            }
            bbCldGroup.SetActive(false);

            for (int i = 0; i < len; ++i)
            {
                Transform parent = curGO.transform;
                if (propertyList[i].IsConvex) parent = parent.Find("PlaneConvexCollider");
                else if (propertyList[i].IsBoundingRect) parent = parent.Find("PlaneBoundingRectCollider");

                if (propertyList[i].IsHorizontal) parent = parent.Find("Horizontal");
                else if (propertyList[i].IsVertical) parent = parent.Find("Vertical");

                if (propertyList[i].IsFrontFace) parent = parent.Find("FrontFace");
                else if (propertyList[i].IsBackFace) parent = parent.Find("BackFace");

                rnds[i].transform.SetParent(parent, true);
                rnds[i].gameObject.AddComponent<MeshCollider>();

                Component.DestroyImmediate(rnds[i]);
            }
        }
    }
}
