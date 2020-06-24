using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPart{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class FogAreaRenderer : MonoBehaviour ,IFogRenderer
    {
        readonly Vector3[] boundsVertices = new Vector3[]{
            new Vector3(0.5f, -0.5f, 0.5f),new Vector3(-0.5f, -0.5f, 0.5f),new Vector3(0.5f, 0.5f, 0.5f),new Vector3(-0.5f, 0.5f, 0.5f),new Vector3(0.5f, 0.5f, -0.5f),new Vector3(-0.5f, 0.5f, -0.5f),new Vector3(0.5f, -0.5f, -0.5f),new Vector3(-0.5f, -0.5f, -0.5f),new Vector3(0.5f, 0.5f, 0.5f),new Vector3(-0.5f, 0.5f, 0.5f),new Vector3(0.5f, 0.5f, -0.5f),new Vector3(-0.5f, 0.5f, -0.5f),new Vector3(0.5f, -0.5f, -0.5f),new Vector3(0.5f, -0.5f, 0.5f),new Vector3(-0.5f, -0.5f, 0.5f),new Vector3(-0.5f, -0.5f, -0.5f),new Vector3(-0.5f, -0.5f, 0.5f),new Vector3(-0.5f, 0.5f, 0.5f),new Vector3(-0.5f, 0.5f, -0.5f),new Vector3(-0.5f, -0.5f, -0.5f),new Vector3(0.5f, -0.5f, -0.5f),new Vector3(0.5f, 0.5f, -0.5f),new Vector3(0.5f, 0.5f, 0.5f),new Vector3(0.5f, -0.5f, 0.5f)
        };
        readonly int[] boundsTraingles = new int[]{
            0,2,3,0,3,1,8,4,5,8,5,9,10,6,7,10,7,11,12,13,14,12,14,15,16,17,18,16,18,19,20,21,22,20,22,23
        };
        [SerializeField,HideInInspector]
        private Mesh mesh_Bounds;
        [SerializeField,HideInInspector]
        private MeshFilter meshFilter;
        [SerializeField,HideInInspector]
        private MeshRenderer meshRenderer;
        public Renderer renderer{
            get{
                return meshRenderer;
            }
        }
        [SerializeField,HideInInspector]
        private FogArea targetFogArea;
        [SerializeField,HideInInspector]
        private Material fogRenderMat;
        void IFogRenderer.InitializeRenderer(FogArea fogArea){
            targetFogArea = fogArea;
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            mesh_Bounds = new Mesh();
            //Calculate bounds;
            var allRenderers = GameObject.FindObjectsOfType<Renderer>();
            Vector3 boundsMin = Vector3.one * Mathf.Infinity;
            Vector3 boundsMax = Vector3.one * Mathf.NegativeInfinity;
            foreach(var item in allRenderers){
                if(item == this.meshRenderer){
                    continue; //ignore self
                }
                boundsMin = MathExternal.Min(boundsMin,item.bounds.min);
                boundsMax = MathExternal.Max(boundsMax,item.bounds.max);
            }
            var center = Vector3.Lerp(boundsMin, boundsMax,0.5f);
            var size = boundsMax - boundsMin;
            List<Vector3> verticles = new List<Vector3>();
            foreach(var item in boundsVertices){
                verticles.Add(transform.InverseTransformPoint((item.Mul(size) + center)));
            }
            mesh_Bounds.SetVertices(verticles);
            mesh_Bounds.SetTriangles(boundsTraingles,0);

            mesh_Bounds.RecalculateTangents();
            mesh_Bounds.RecalculateNormals();
            mesh_Bounds.RecalculateBounds();
            verticles = null;

            meshFilter.mesh = mesh_Bounds;
            if(fogRenderMat == null){
                fogRenderMat = new Material(Shader.Find("ThirdPart/FogRenderer/Projected"));
                meshRenderer.material = fogRenderMat;
            }
        }

    }

    public static class MathExternal{
        public static Vector3 Abs(Vector3 v0){
            return new Vector3(
                Mathf.Abs(v0.x),
                Mathf.Abs(v0.y),
                Mathf.Abs(v0.z)
            );
        }
        public static Vector3 Max(Vector3 v0, Vector3 v1){
            return new Vector3(
                Mathf.Max(v0.x,v1.x),
                Mathf.Max(v0.y,v1.y),
                Mathf.Max(v0.z,v1.z)
            );
        }
        public static Vector3 Min(Vector3 v0, Vector3 v1){
            return new Vector3(
                Mathf.Min(v0.x,v1.x),
                Mathf.Min(v0.y,v1.y),
                Mathf.Min(v0.z,v1.z)
            );
        }
        public static Vector3 Mul(this Vector3 v0, Vector3 v1){
            return new Vector3(
                (v0.x * v1.x),
                (v0.y * v1.y),
                (v0.z * v1.z)
            );
        }
    }
}
