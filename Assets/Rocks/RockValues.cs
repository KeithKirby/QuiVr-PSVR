using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rocks", menuName = "RockData", order = 1)]
public class RockValues : ScriptableObject {

    public PhysicMaterial RockMat;
    public Rock[] Rocks;

    [System.Serializable]
    public class Rock
    {
        public RockMesh[] Meshes;
        
        public Rock()
        {
            Meshes = new RockMesh[5];
            for (int i = 0; i < 5; i++)
                Meshes[i] = new RockMesh();
            Meshes[0].Side = RockSide.Base;
            Meshes[1].Side = RockSide.Front;
            Meshes[2].Side = RockSide.Back;
            Meshes[3].Side = RockSide.Side1;
            Meshes[4].Side = RockSide.Side2;
        }

        public override string ToString()
        {
            if (Meshes != null && Meshes.Length > 0 && Meshes[0] != null)
                return Meshes[0].ToString();
            return "Not Set";
        }

        [AdvancedInspector.Inspect]
        public void Populate()
        {
#if UNITY_EDITOR
            if (Meshes[0].BaseMesh != null)
            {
                string meshPath = UnityEditor.AssetDatabase.GetAssetPath(Meshes[0].BaseMesh);
                for(int i=1; i<5; i++)
                {
                    string[] pts = meshPath.Split('.');
                    pts[0] = pts[0].Replace("_FBX", "");
                    string path = pts[0] + "_" + ((RockSide)i).ToString() + "." + pts[1];
                    Mesh msh = (Mesh)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
                    Meshes[i].BaseMesh = msh;
                }
                //meshPath = meshPath.Replace("/"+Meshes[0].BaseMesh.ToString().Split(' ')[0]+".FBX", "");

            }
            else
                Debug.Log("Base Mesh Not Set - Aborting");
#endif
        }
    }

    public enum RockSide
    {
        Base,
        Front,
        Back,
        Side1,
        Side2
    }

    [System.Serializable]
    public class RockMesh
    {
        public RockSide Side;
        public Mesh BaseMesh;
        public Mesh[] LODS;

        public override string ToString()
        {
            if(BaseMesh != null)
             return Side.ToString() + "_" + BaseMesh.ToString().Split(' ')[0];
            return "None";
        }
    }
}
