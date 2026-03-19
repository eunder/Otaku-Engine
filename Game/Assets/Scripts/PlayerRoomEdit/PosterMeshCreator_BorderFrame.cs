using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PosterMeshCreator_BorderFrame : MonoBehaviour
{
    public Material outsideFrameMaterial;
    public Material innerRimMaterial;
    public Color[] rimColors = new Color[]{Color.black, Color.black};
    public float[] rimLuminance = new float[]{0.0f,0.0f};

     Vector3[] rimVertices;

  //  [Range(0.01f, 0.20f)]
    public float frameThickness = 0.02f;
    public float frame_width = 0.02f;
    public float frame_height = 0.02f;

    public MeshFilter meshOfObjectToDrawFrameAround;
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    PosterMeshCreator basePosterMesh;
    public bool isLastFrame;
    
    public float heightDepth = 0.0f;

    public Material[] materialArray;
     public void Start()
    {
       if(!GetComponent<MeshRenderer>())
       {
       meshRenderer = gameObject.AddComponent<MeshRenderer>();
       }
       else
       {
       meshRenderer = gameObject.GetComponent<MeshRenderer>();
       }


       if(!GetComponent<MeshFilter>())
       {
       meshFilter = gameObject.AddComponent<MeshFilter>();
       }
       else
       {
       meshFilter = gameObject.GetComponent<MeshFilter>();
       }

        if(mesh == null)
            {
                mesh = new Mesh {
                    name = "Procedural Mesh"
                };
            }
    BuildFrame();
    }
    Vector3[] vertices;

    public void BuildFrame()
    {
       basePosterMesh = GetComponentInParent<PosterMeshCreator>();

                if(isLastFrame != true)
        {
        vertices = meshOfObjectToDrawFrameAround.sharedMesh.vertices;

        //assign colors
         gameObject.GetComponent<Renderer>().materials[0].color = rimColors[0];
         gameObject.GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", rimColors[0] * rimLuminance[0]);
         gameObject.GetComponent<Renderer>().materials[1].color = rimColors[1];
         gameObject.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", rimColors[1] * rimLuminance[1]);

        //draw first test border rim
         rimVertices = new Vector3[12]
        {
            //outer vertices
            vertices[0] + new Vector3(frame_width,heightDepth ,frame_height),
            vertices[1] + new Vector3(frame_width,heightDepth,-frame_height),
            vertices[2] + new Vector3(-frame_width,heightDepth,-frame_height),
            vertices[3] + new Vector3(-frame_width,heightDepth,frame_height),

            //inner vertices
            vertices[0] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[1] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[2] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[3] + new Vector3(0.0f, heightDepth,0.0f),

            // deep rim vertices
            vertices[0],  //8
            vertices[1],  //9
            vertices[2],
            vertices[3]

            /*
            vertices[0] + new Vector3(0.0f,-0.04f,0.0f),
            vertices[1] + new Vector3(0.0f,-0.04f,0.0f),
            vertices[2] + new Vector3(0.0f,-0.04f,0.0f),
            vertices[3] + new Vector3(0.0f,-0.04f,0.0f),    // since the frame is 0.4f from the ground
            */
        };
         mesh.vertices = rimVertices;


        int[] tris = new int[]
        {
            //right side
            0, 1, 4,
            4, 1, 5,

            //bottom side
            5, 1, 2,
            2, 6, 5,

            2, 3, 6,
            3, 7, 6,

            3, 0, 7,
            7, 0, 4,
        };


        int[] tris2 = new int[]
        {
            // heightdepth triangles

            //right inner
            4, 5, 8,
            5, 9, 8,
            //bottom inner
            5, 6, 9,
            6, 10, 9,
            // left inner
            6, 7, 10,
            7, 11, 10,
            //upper inner
            7, 4, 11,
            4, 8, 11
        };
		mesh.subMeshCount = 2;

        mesh.SetTriangles(tris, 0);
        mesh.SetTriangles(tris2, 1);


        Vector3[] normals = new Vector3[12]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[12]
        {
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };
        mesh.uv = uv;
        meshFilter.mesh = mesh;

        }
        else
        {

                float totalAddedDepth = 0.0f;
                for(int i = 0; i <basePosterMesh.GetComponent<PosterFrameList>().posterFrameList.Count - 1; i++)
                {
                    totalAddedDepth += basePosterMesh.GetComponent<PosterFrameList>().posterFrameList[i].GetComponent<PosterMeshCreator_BorderFrame>().heightDepth;
                }
                
        vertices = meshOfObjectToDrawFrameAround.sharedMesh.vertices;
        
        //assign colors
         gameObject.GetComponent<Renderer>().materials[0].color = rimColors[0];
         gameObject.GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", rimColors[0] * rimLuminance[0]);
         gameObject.GetComponent<Renderer>().materials[1].color = rimColors[1];
         gameObject.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", rimColors[1] * rimLuminance[1]);

        //draw first test border rim
         rimVertices = new Vector3[16]
        {
                        

            vertices[0] + new Vector3(frame_width,-totalAddedDepth + (-basePosterMesh.distanceFromWall),frame_height),
            vertices[1] + new Vector3(frame_width,-totalAddedDepth + (-basePosterMesh.distanceFromWall),-frame_height),
            vertices[2] + new Vector3(-frame_width,-totalAddedDepth + (-basePosterMesh.distanceFromWall),-frame_height),
            vertices[3] + new Vector3(-frame_width,-totalAddedDepth + (-basePosterMesh.distanceFromWall),frame_height),    // since the frame is 0.4f from the ground
            

            //outer vertices
            vertices[0] + new Vector3(frame_width,heightDepth ,frame_height),
            vertices[1] + new Vector3(frame_width,heightDepth,-frame_height),
            vertices[2] + new Vector3(-frame_width,heightDepth,-frame_height),
            vertices[3] + new Vector3(-frame_width,heightDepth,frame_height),

            //inner vertices
            vertices[0] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[1] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[2] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[3] + new Vector3(0.0f, heightDepth,0.0f),

            // deep rim vertices
            vertices[0],  //8
            vertices[1],  //9
            vertices[2],
            vertices[3]


        };

        if(mesh)
         mesh.vertices = rimVertices;


        int[] tris = new int[]
        {
            //right side
            0, 1, 5,
            5, 4, 0,

            //bottom side
            5, 1, 2,
            2, 6, 5,

            6, 2, 3,
            3, 7, 6,

            0, 4, 7,
            7, 3, 0,



            //right side
            4, 5, 8,
            8, 5, 9,

            //bottom side
            9, 5, 6,
            6, 10, 9,

            10, 6, 7,
            7, 11, 10,

            11, 7, 4,
            4, 8, 11,
        };


        int[] tris2 = new int[]
        {
            // heightdepth triangles

            //right inner
            8, 9, 13,
            13, 12, 8,
            //bottom inner
            9, 10, 13,
            13, 10, 14,
            // left inner
            15, 14, 10,
            10, 11, 15,
            //upper inner
            11, 8, 15,
            15, 8, 12
        };

        if(mesh)
        {
		mesh.subMeshCount = 2;
        mesh.SetTriangles(tris, 0);
        mesh.SetTriangles(tris2, 1);
        }

        Vector3[] normals = new Vector3[16]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };

        if(mesh)
        mesh.normals = normals;

        Vector2[] uv = new Vector2[16]
        {
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };

        if(mesh)
        {
        mesh.uv = uv;
        meshFilter.mesh = mesh;
        }
        }
                if(mesh)
        meshFilter.mesh.RecalculateBounds();

    }

    void Update()
    {
       // UpdateFrame();
    }
    public void UpdateFrame()
    {
                 if(isLastFrame != true)
        {
      if(!GetComponent<MeshRenderer>())
                {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
                }
                else
                {
                meshRenderer = gameObject.GetComponent<MeshRenderer>();
                }

                if(!GetComponent<MeshFilter>())
                    {
                    meshFilter = gameObject.AddComponent<MeshFilter>();
                    }
                    else
                    {
                    meshFilter = gameObject.GetComponent<MeshFilter>();
                    }
                if(mesh != null)
                {
                    Destroy(mesh);
                }
                        mesh = new Mesh {
                            name = "Procedural Mesh"
                        };



        vertices = meshOfObjectToDrawFrameAround.sharedMesh.vertices;

        //assign colors
         gameObject.GetComponent<Renderer>().materials[0].color = rimColors[0];
         gameObject.GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", rimColors[0] * rimLuminance[0]);
         gameObject.GetComponent<Renderer>().materials[1].color = rimColors[1];
         gameObject.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", rimColors[1] * rimLuminance[1]);

        //draw first test border rim
         rimVertices = new Vector3[12]
        {
            //outer vertices
            vertices[0] + new Vector3(frame_width,heightDepth ,frame_height),
            vertices[1] + new Vector3(frame_width,heightDepth,-frame_height),
            vertices[2] + new Vector3(-frame_width,heightDepth,-frame_height),
            vertices[3] + new Vector3(-frame_width,heightDepth,frame_height),

            //inner vertices
            vertices[0] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[1] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[2] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[3] + new Vector3(0.0f, heightDepth,0.0f),

            // deep rim vertices
            vertices[0],  //8
            vertices[1],  //9
            vertices[2],
            vertices[3]

            /*
            vertices[0] + new Vector3(0.0f,-0.04f,0.0f),
            vertices[1] + new Vector3(0.0f,-0.04f,0.0f),
            vertices[2] + new Vector3(0.0f,-0.04f,0.0f),
            vertices[3] + new Vector3(0.0f,-0.04f,0.0f),    // since the frame is 0.4f from the ground
            */
        };
         mesh.vertices = rimVertices;


        int[] tris = new int[]
        {
            //right side
            0, 1, 4,
            4, 1, 5,

            //bottom side
            5, 1, 2,
            2, 6, 5,

            2, 3, 6,
            3, 7, 6,

            3, 0, 7,
            7, 0, 4,
        };


        int[] tris2 = new int[]
        {
            // heightdepth triangles

            //right inner
            4, 5, 8,
            5, 9, 8,
            //bottom inner
            5, 6, 9,
            6, 10, 9,
            // left inner
            6, 7, 10,
            7, 11, 10,
            //upper inner
            7, 4, 11,
            4, 8, 11
        };
		mesh.subMeshCount = 2;

        mesh.SetTriangles(tris, 0);
        mesh.SetTriangles(tris2, 1);


        Vector3[] normals = new Vector3[12]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[12]
        {
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };
        mesh.uv = uv;
        meshFilter.mesh = mesh;
        meshFilter.sharedMesh.RecalculateBounds();
        }
        else
        {

                            if(!GetComponent<MeshRenderer>())
                {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
                }
                else
                {
                meshRenderer = gameObject.GetComponent<MeshRenderer>();
                }

                if(!GetComponent<MeshFilter>())
                    {
                    meshFilter = gameObject.AddComponent<MeshFilter>();
                    }
                    else
                    {
                    meshFilter = gameObject.GetComponent<MeshFilter>();
                    }

                if(mesh != null)
                {
                    Destroy(mesh);
                }
                        mesh = new Mesh {
                            name = "Procedural Mesh"
                        };


                float totalAddedDepth = 0.0f;

                if(basePosterMesh == null)
                {
                    basePosterMesh = GetComponentInParent<PosterMeshCreator>();
                }

                for(int i = 0; i <basePosterMesh.GetComponent<PosterFrameList>().posterFrameList.Count - 1; i++)
                {
                    totalAddedDepth += basePosterMesh.GetComponent<PosterFrameList>().posterFrameList[i].GetComponent<PosterMeshCreator_BorderFrame>().heightDepth;
                }
                
        vertices = meshOfObjectToDrawFrameAround.sharedMesh.vertices;
        
        //assign colors
         gameObject.GetComponent<Renderer>().materials[0].color = rimColors[0];
         gameObject.GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", rimColors[0] * rimLuminance[0]);
         gameObject.GetComponent<Renderer>().materials[1].color = rimColors[1];
         gameObject.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", rimColors[1] * rimLuminance[1]);





        //draw first test border rim
         rimVertices = new Vector3[16]
        {
                        

            vertices[0] + new Vector3(frame_width,-totalAddedDepth + (-basePosterMesh.distanceFromWall),frame_height),
            vertices[1] + new Vector3(frame_width,-totalAddedDepth + (-basePosterMesh.distanceFromWall),-frame_height),
            vertices[2] + new Vector3(-frame_width,-totalAddedDepth + (-basePosterMesh.distanceFromWall),-frame_height),
            vertices[3] + new Vector3(-frame_width,-totalAddedDepth + (-basePosterMesh.distanceFromWall),frame_height),    // since the frame is 0.4f from the ground
            

            //outer vertices
            vertices[0] + new Vector3(frame_width,heightDepth ,frame_height),
            vertices[1] + new Vector3(frame_width,heightDepth,-frame_height),
            vertices[2] + new Vector3(-frame_width,heightDepth,-frame_height),
            vertices[3] + new Vector3(-frame_width,heightDepth,frame_height),

            //inner vertices
            vertices[0] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[1] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[2] + new Vector3(0.0f, heightDepth,0.0f),
            vertices[3] + new Vector3(0.0f, heightDepth,0.0f),

            // deep rim vertices
            vertices[0],  //8
            vertices[1],  //9
            vertices[2],
            vertices[3]


        };
         mesh.vertices = rimVertices;


        int[] tris = new int[]
        {
            //right side
            0, 1, 5,
            5, 4, 0,

            //bottom side
            5, 1, 2,
            2, 6, 5,

            6, 2, 3,
            3, 7, 6,

            0, 4, 7,
            7, 3, 0,



            //right side
            4, 5, 8,
            8, 5, 9,

            //bottom side
            9, 5, 6,
            6, 10, 9,

            10, 6, 7,
            7, 11, 10,

            11, 7, 4,
            4, 8, 11,
        };


        int[] tris2 = new int[]
        {
            // heightdepth triangles

            //right inner
            8, 9, 13,
            13, 12, 8,
            //bottom inner
            9, 10, 13,
            13, 10, 14,
            // left inner
            15, 14, 10,
            10, 11, 15,
            //upper inner
            11, 8, 15,
            15, 8, 12
        };
		mesh.subMeshCount = 2;

        mesh.SetTriangles(tris, 0);
        mesh.SetTriangles(tris2, 1);


        Vector3[] normals = new Vector3[16]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[16]
        {
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };
        mesh.uv = uv;
        meshFilter.sharedMesh = mesh;
        meshFilter.sharedMesh.RecalculateBounds();

        
        }

    }
    
    #if UNITY_EDITOR

        void OnDrawGizmosSelected()
    {
        
        //draw test rim border vertices
                int i2 = 0;
         foreach (Vector3 vertex in rimVertices) {
        Vector3 vertexWorldPos = gameObject.transform.TransformPoint(vertex);
        Handles.Label(vertexWorldPos, i2.ToString());
        i2++;
          }  
        
    }
    #endif
}
