using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;

public class PosterFrameCreator_AreaPreview : MonoBehaviour
{    public Material testMat;
     public PosterMeshCreator posterMeshCreator;
    public Vector3[] vertices;
    Mesh mesh;
    void Start()
    {
        if(!mesh)
        {
        BuildPoster();
        }
    }

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    
    public float distanceFromWall = 0f;
    public float distanceFromWall_base = 0.04f; //small for posters(0.002), a litte bigger for frames(0.04)
    public void BuildPoster()
    {
        if(!mesh)
        {
        mesh = new Mesh {
			name = "Procedural Mesh"
		};
        }

        if(!GetComponent<MeshRenderer>())
        {
         meshRenderer = gameObject.AddComponent<MeshRenderer>();
         meshRenderer.sharedMaterial = testMat;
        }
        if(!GetComponent<MeshFilter>())
        {
         meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        else
        {
         meshFilter = gameObject.GetComponent<MeshFilter>();
        }
        
        // THE EXTRA FOUR(4) THINGS USED(verticy, nomal, and uv) IS TO GIVE THE MESH SOME DEPTH FOR THE MESH CONVEX COLLIDER

        vertices = new Vector3[4]
        {                               //y axis controlls how far from the ground the image is!!!    // 0.4f from the ground
            new Vector3(0f,0f,0f),
            new Vector3(0f,0f,0f),
            new Vector3(0f,0f,0f),
            new Vector3(0f,0f,0f)
        };
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
     }
    public float scaleOffset = 1.0f;

     void Update() //OPTIMIZE THISS!!!
     {
         distanceFromWall = distanceFromWall_base / scaleOffset;
        gameObject.transform.localScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);


    if(posterMeshCreator != null)
    {
        vertices = new Vector3[4]
        {                               //y axis controlls how far from the ground the image is!!!    // 0.4f from the ground
            new Vector3(posterMeshCreator.box.x, distanceFromWall, posterMeshCreator.box.y),
            new Vector3(posterMeshCreator.box.x, distanceFromWall, -posterMeshCreator.box.y),
            new Vector3(-posterMeshCreator.box.x, distanceFromWall, -posterMeshCreator.box.y),
            new Vector3(-posterMeshCreator.box.x, distanceFromWall, posterMeshCreator.box.y)
        };
        mesh.vertices = vertices;
        meshFilter.mesh = mesh;
    }
     }

 }
