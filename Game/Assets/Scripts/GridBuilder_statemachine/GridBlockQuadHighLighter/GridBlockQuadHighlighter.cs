using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class GridBlockQuadHighlighter : MonoBehaviour
{

    public GameObject camPlayer;
    public Material previewMat;
    public GridBuilderStateMachine gridBuilderStateMachine;

    public Vector3[] vertices;
    MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    MeshCollider meshCollider;
    
    public int v0Index;
    public int v1Index;
    public int v2Index;
    public int v3Index;

    public int currentFaceHighlighting = 0;
    public int currentMaterialIndexHighlighting = 0;
    public GameObject currenGameObjectHitting;
    public bool preventFromHighlightingNewFace = false;

    public LayerMask layerMask;

    Mesh mesh;

    public void BuildPoster()
    {
        if(!preventFromHighlightingNewFace)
        {
        if(!GetComponent<MeshRenderer>())
        {
         meshRenderer = gameObject.AddComponent<MeshRenderer>();
         meshRenderer.sharedMaterial = previewMat;
        }
        if(!GetComponent<MeshFilter>())
        {
         meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        else
        {
         meshFilter = gameObject.GetComponent<MeshFilter>();
        }

        if(!mesh)
        {
        mesh = new Mesh {
			name = "Procedural Mesh"
		};
        }

               Ray ray = gridBuilderStateMachine.mainCam.ScreenPointToRay(Input.mousePosition);
               RaycastHit hit = new RaycastHit();
     if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
     {
                  Debug.DrawRay(camPlayer.transform.position, camPlayer.transform.forward * hit.distance, Color.cyan);
         int ind = hit.triangleIndex;
         Debug.Log("Hit tri index is " + ind);
         currenGameObjectHitting = hit.transform.gameObject;

    if(ind == 0 || ind == 1)
    {
     v0Index = 0;
     v1Index = 1;
     v2Index = 2;
     v3Index = 3;
     currentFaceHighlighting = 0;
     currentMaterialIndexHighlighting = 0;
    }
    if(ind == 2 || ind == 3)
    {
     v0Index = 4;
     v1Index = 5;
     v2Index = 6;
     v3Index = 7;
     currentFaceHighlighting = 1;
     currentMaterialIndexHighlighting = 1;
    }
    if(ind == 4 || ind == 5)
    {
     v0Index = 8;
     v1Index = 9;
     v2Index = 10;
     v3Index = 11;
     currentFaceHighlighting = 2;
     currentMaterialIndexHighlighting = 2;
    }
    if(ind == 6 || ind == 7)
    {
     v0Index = 12;
     v1Index = 13;
     v2Index = 14;
     v3Index = 15;
     currentFaceHighlighting = 3;
     currentMaterialIndexHighlighting = 3;
    }
    if(ind == 8 || ind == 9)
    {
     v0Index = 16;
     v1Index = 17;
     v2Index = 18;
     v3Index = 19;
     currentFaceHighlighting = 4;
     currentMaterialIndexHighlighting = 4;  
    }
    if(ind == 10 || ind == 11)
    {
     v0Index = 20;
     v1Index = 21;
     v2Index = 22;
     v3Index = 23;
     currentFaceHighlighting = 5;
     currentMaterialIndexHighlighting = 5;
    }
        vertices = new Vector3[4]
        {                         
            hit.transform.TransformPoint(hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[v0Index]),
            hit.transform.TransformPoint(hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[v1Index]),
            hit.transform.TransformPoint(hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[v2Index]),
            hit.transform.TransformPoint(hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[v3Index])
         };

     }
     else
     {
         currenGameObjectHitting = null;
        vertices = new Vector3[4]
        {                         
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero
        };
     }



        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // upper left triangle
            0, 1, 2,
            // lower right triangle
            2, 3, 0

        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,    
            Vector3.forward
                };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;
        meshFilter.sharedMesh.RecalculateBounds();
        meshFilter.sharedMesh.RecalculateNormals();

        }
     }

     void Update()
     {
             BuildPoster();
     }

}
