 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class BlockFaceTextureUVProperties : MonoBehaviour
{
    public GameObject parent;
    public MeshFilter cubeMesh;
    public string materialName_y;
    public string materialName_yneg;
    public string materialName_x;
    public string materialName_xneg;
    public string materialName_z;
    public string materialName_zneg;
    
    public Color materialColor_y;
    public Color materialColor_yneg;
    public Color materialColor_x;
    public Color materialColor_xneg;
    public Color materialColor_z;
    public Color materialColor_zneg;


    private MaterialPropertyBlock propBlock_y;
    private MaterialPropertyBlock propBlock_yneg;
    private MaterialPropertyBlock propBlock_x;
    private MaterialPropertyBlock propBlock_xneg;
    private MaterialPropertyBlock propBlock_z;
    private MaterialPropertyBlock propBlock_zneg;



    
    public Renderer _renderer;

    public Vector2[] UVOffSet = new Vector2[6]{     //one for each face
    new Vector2 (0.0f,0.0f), //top
    new Vector2 (0.0f,0.0f), //x+ side
    new Vector2 (0.0f,0.0f), //bottom
    new Vector2 (0.0f,0.0f), //x- side
    new Vector2 (0.0f,0.0f), //z- side
    new Vector2 (0.0f,0.0f)  //z+ side
    }; 

    public Vector2[] UVScale = new Vector2[6]{     //one for each face
    new Vector2 (1.0f,1.0f), //top
    new Vector2 (1.0f,1.0f), //x+ side
    new Vector2 (1.0f,1.0f), //bottom
    new Vector2 (1.0f,1.0f), //x- side
    new Vector2 (1.0f,1.0f), //z- side
    new Vector2 (1.0f,1.0f)  //z+ side
    }; 

    public float[] UVRotation = new float[6]{ //one for each face
    0, //top
    0, //x+ side
    0, //bottom
    0, //x- side
    0, //z- side
    0  //z+ side
    };

    void Start()
    {
        AssignDevPosterMaterials();

        UpdateBlockUV();


        CheckIfPropertyBlocksExist();


            //to prevent errors on object duplicate
            SetMaterialColorBasedOnIndex(0,materialColor_y);
            SetMaterialColorBasedOnIndex(1,materialColor_x);
            SetMaterialColorBasedOnIndex(2,materialColor_yneg);
            SetMaterialColorBasedOnIndex(3,materialColor_xneg);
            SetMaterialColorBasedOnIndex(4,materialColor_zneg);
            SetMaterialColorBasedOnIndex(5,materialColor_z);


        if(_renderer == null)
        {
        _renderer = GetComponent<Renderer>();
        }

 
    }

    //this makes sure everytime a new block is created. the appropriate GUID of the "dev_sides" and "dev_top" posters gets assigned to the faces
    void AssignDevPosterMaterials()
    {
        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster != null)
            {
                //check if there is a poster with the dev name... if there is... then check if the block does not have a materialName already assigned...

                if(poster.GetComponent<Note>().note == "dev_sides")
                {
                    if(string.IsNullOrWhiteSpace(materialName_x))
                    {
                        ChangeFaceMatName(1 ,poster.name);
                    }
                    if(string.IsNullOrWhiteSpace(materialName_xneg))
                    {
                        ChangeFaceMatName(3 ,poster.name);
                    }
                    if(string.IsNullOrWhiteSpace(materialName_zneg))
                    {
                        ChangeFaceMatName(4 ,poster.name);
                    }
                    if(string.IsNullOrWhiteSpace(materialName_z))
                    {
                        ChangeFaceMatName(5 ,poster.name);
                    }
                }
                if(poster.GetComponent<Note>().note == "dev_top")
                {
                    if(string.IsNullOrWhiteSpace(materialName_y))
                    {
                        ChangeFaceMatName(0 ,poster.name);
                    }
                    if(string.IsNullOrWhiteSpace(materialName_yneg))
                    {
                        ChangeFaceMatName(2 ,poster.name);
                    }
                }
            }
        }

        AssignSharedMaterialBasedOnGUID();
    }



    //DEBUGGING ONLY

    /*
    void Update()
    {
        UpdateBlockUV();
      //  Debug.DrawRay(transform.TransformPoint(originalPivot), transform.up * 2.0f, Color.red);
    }
    */

    private void CheckIfPropertyBlocksExist()
    {
        if(propBlock_y == null)
        {
            propBlock_y = new MaterialPropertyBlock();
        }
        if(propBlock_yneg == null)
        {
            propBlock_yneg = new MaterialPropertyBlock();
        }
        if(propBlock_x == null)
        {
            propBlock_x = new MaterialPropertyBlock();
        }
        if(propBlock_xneg == null)
        {
            propBlock_xneg = new MaterialPropertyBlock();
        }
        if(propBlock_zneg == null)
        {
            propBlock_z = new MaterialPropertyBlock();
        }
        if(propBlock_zneg == null)
        {
            propBlock_zneg = new MaterialPropertyBlock();
        }
    }



    private Vector3[] verts;

    public SaveAndLoadLevel.Corner closestCornerPoint;
    public SaveAndLoadLevel.Corner secondclosestCornerPoint;

    //used as the invinsible "base", for texturing. Because the pivot gets reset everytime the block is modified.
    //without this... the texturing would get messed up... because the offsets wouldnt be correct
    public Vector3 originalPivot; //used is a local pos


    //CORNERS




                //top 
                public SaveAndLoadLevel.Corner corner_Y_X_Z ;
                public SaveAndLoadLevel.Corner corner_Y_X_Zneg ;
                public SaveAndLoadLevel.Corner corner_Y_Xneg_Z ;
                public SaveAndLoadLevel.Corner corner_Y_Xneg_Zneg ;

                //bottom
                public SaveAndLoadLevel.Corner corner_Yneg_X_Z;
                public SaveAndLoadLevel.Corner corner_Yneg_X_Zneg ;
                public SaveAndLoadLevel.Corner corner_Yneg_Xneg_Z ;
                public SaveAndLoadLevel.Corner corner_Yneg_Xneg_Zneg;


    //CORNER OFFSETS
                public Vector3 corner_Y_X_Z_offset;
                public Vector3 corner_Y_X_Zneg_offset;
                public Vector3 corner_Y_Xneg_Z_offset;
                public Vector3 corner_Y_Xneg_Zneg_offset;

                
                public Vector3 corner_Yneg_X_Z_offset;
                public Vector3 corner_Yneg_X_Zneg_offset;
                public Vector3 corner_Yneg_Xneg_Z_offset;
                public Vector3 corner_Yneg_Xneg_Zneg_offset;



        public void CornerClosestOffsetToEditToRayHit(Vector3 hitPos)
    {

        //TODO
        //get corner positions(not the start, and not the offset!)
        //make sure to convert to world space

                //top 
                 corner_Y_X_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[2]);
                 corner_Y_X_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[3]);
                 corner_Y_Xneg_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[1]);
                 corner_Y_Xneg_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[0]);

                //bottom
                 corner_Yneg_X_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[4]);
                 corner_Yneg_X_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[5]);
                 corner_Yneg_Xneg_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[10]);
                 corner_Yneg_Xneg_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[11]);



        SaveAndLoadLevel.Corner[] corners = {corner_Y_X_Z, corner_Y_X_Zneg, corner_Y_Xneg_Z, corner_Y_Xneg_Zneg, 
                                   corner_Yneg_X_Z, corner_Yneg_X_Zneg, corner_Yneg_Xneg_Z, corner_Yneg_Xneg_Zneg};

        float closestDist = Mathf.Infinity;


        foreach(SaveAndLoadLevel.Corner corner in corners)
        {
            float distance = Vector3.Distance(corner.corner_Pos, hitPos);

            if(distance < closestDist)
            {
                closestCornerPoint = corner;
                closestDist = distance;
            }
       }
    }


public SaveAndLoadLevel.Corner[] GetCurrentListOfUpdatedCorners()
{
                //NOTE: this function return a brand new set of corner objects.. if you dont create a new set... then it will return references...

                SaveAndLoadLevel.Corner new_corner_Y_X_Z = new SaveAndLoadLevel.Corner();
                SaveAndLoadLevel.Corner new_corner_Y_X_Zneg = new SaveAndLoadLevel.Corner();
                SaveAndLoadLevel.Corner new_corner_Y_Xneg_Z = new SaveAndLoadLevel.Corner();
                SaveAndLoadLevel.Corner new_corner_Y_Xneg_Zneg = new SaveAndLoadLevel.Corner();

                SaveAndLoadLevel.Corner new_corner_Yneg_X_Z = new SaveAndLoadLevel.Corner();
                SaveAndLoadLevel.Corner new_corner_Yneg_X_Zneg = new SaveAndLoadLevel.Corner();
                SaveAndLoadLevel.Corner new_corner_Yneg_Xneg_Z = new SaveAndLoadLevel.Corner();
                SaveAndLoadLevel.Corner new_corner_Yneg_Xneg_Zneg = new SaveAndLoadLevel.Corner();

                    //top 
                 new_corner_Y_X_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[2]);
                 new_corner_Y_X_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[3]);
                 new_corner_Y_Xneg_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[1]);
                 new_corner_Y_Xneg_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[0]);

                //bottom
                 new_corner_Yneg_X_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[4]);
                 new_corner_Yneg_X_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[5]);
                 new_corner_Yneg_Xneg_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[10]);
                 new_corner_Yneg_Xneg_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[11]);

                SaveAndLoadLevel.Corner[] corners = {new_corner_Y_X_Z, new_corner_Y_X_Zneg, new_corner_Y_Xneg_Z, new_corner_Y_Xneg_Zneg, 
                new_corner_Yneg_X_Z, new_corner_Yneg_X_Zneg, new_corner_Yneg_Xneg_Z, new_corner_Yneg_Xneg_Zneg};

                return corners;
}

public void SetCornersFromCornerList(SaveAndLoadLevel.Corner[] corners)
{  
        corner_Y_X_Z.corner_Pos = corners[0].corner_Pos;
        corner_Y_X_Zneg.corner_Pos = corners[1].corner_Pos;
        corner_Y_Xneg_Z.corner_Pos = corners[2].corner_Pos;
        corner_Y_Xneg_Zneg.corner_Pos = corners[3].corner_Pos;

        corner_Yneg_X_Z.corner_Pos = corners[4].corner_Pos;
        corner_Yneg_X_Zneg.corner_Pos = corners[5].corner_Pos;
        corner_Yneg_Xneg_Z.corner_Pos = corners[6].corner_Pos;
        corner_Yneg_Xneg_Zneg.corner_Pos = corners[7].corner_Pos;
    

    UpdateCorners();
}


//Two Cloest Corners
 public void TwoClosestCornersRayHit(Vector3 hitPos)
    {


        //TODO
        //get corner positions(not the start, and not the offset!)
        //make sure to convert to world space

                //top 
                 corner_Y_X_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[2]);
                 corner_Y_X_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[3]);
                 corner_Y_Xneg_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[1]);
                 corner_Y_Xneg_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[0]);

                //bottom
                 corner_Yneg_X_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[4]);
                 corner_Yneg_X_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[5]);
                 corner_Yneg_Xneg_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[10]);
                 corner_Yneg_Xneg_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[11]);



        List<SaveAndLoadLevel.Corner> corners = new List<SaveAndLoadLevel.Corner>
        {corner_Y_X_Z, corner_Y_X_Zneg, corner_Y_Xneg_Z, corner_Y_Xneg_Zneg,corner_Yneg_X_Z, corner_Yneg_X_Zneg, corner_Yneg_Xneg_Z, corner_Yneg_Xneg_Zneg};

        //This uses a special class (VertexDistanceComparer) to sort by distance (0 = closest)
        VertexDistanceComparer comparer = new VertexDistanceComparer(hitPos);
        corners.Sort(comparer);

        closestCornerPoint = corners[0];
        secondclosestCornerPoint = corners[1];
        Debug.Log("cloest" + closestCornerPoint.corner_Pos);
        Debug.Log("second" + secondclosestCornerPoint.corner_Pos);
    }





    //In the following two functions You can just use something similar to the GridBlockQuadHighlighter vertex picking system. Where you do a check for every possible trinagle index
    public void NearestEdgeFromHitPoint(int triangleIndex, Vector3 rayHitPoint)
    {
        if(triangleIndex == 0 || triangleIndex == 1)
        {
            NearestEdgeFromFourVerticesAndHitPoint(0,1,2,3, rayHitPoint);
        }
        if(triangleIndex == 2 || triangleIndex == 3)
        {
            NearestEdgeFromFourVerticesAndHitPoint(4,5,6,7, rayHitPoint);
        }
        if(triangleIndex == 4 || triangleIndex == 5)
        {
            NearestEdgeFromFourVerticesAndHitPoint(8,9,10,11, rayHitPoint);
        }
        if(triangleIndex == 6 || triangleIndex == 7)
        {
            NearestEdgeFromFourVerticesAndHitPoint(12,13,14,15, rayHitPoint);
        }
        if(triangleIndex == 8 || triangleIndex == 9)
        {
            NearestEdgeFromFourVerticesAndHitPoint(16,17,18,19, rayHitPoint);
        }
        if(triangleIndex == 10 || triangleIndex == 11)
        {
            NearestEdgeFromFourVerticesAndHitPoint(20,21,22,23, rayHitPoint);
        }

    }


    public Vector3 vertex1PosOfClosestEdge;
    public Vector3 vertex2PosOfClosestEdge;
    
    //finds the two positions needed in order
    public void NearestEdgeFromFourVerticesAndHitPoint(int v1, int v2, int v3, int v4, Vector3 rayHitPoint)
    {
        //due to the way the mesh is built, you can easily just find the edges with: (v1/v2) (v2/3) (v3/v4) (v4/v1)


        //find the closest edge point from the ray for ALL edges.
        Vector3 edge1ClosestPointToRay = GetClosestPointOnEdgeFromRayHitAndTwoPositions(transform.TransformPoint(cubeMesh.mesh.vertices[v1]), transform.TransformPoint(cubeMesh.mesh.vertices[v2]), rayHitPoint);
        Vector3 edge2ClosestPointToRay = GetClosestPointOnEdgeFromRayHitAndTwoPositions(transform.TransformPoint(cubeMesh.mesh.vertices[v2]), transform.TransformPoint(cubeMesh.mesh.vertices[v3]), rayHitPoint);
        Vector3 edge3ClosestPointToRay = GetClosestPointOnEdgeFromRayHitAndTwoPositions(transform.TransformPoint(cubeMesh.mesh.vertices[v3]), transform.TransformPoint(cubeMesh.mesh.vertices[v4]), rayHitPoint);
        Vector3 edge4ClosestPointToRay = GetClosestPointOnEdgeFromRayHitAndTwoPositions(transform.TransformPoint(cubeMesh.mesh.vertices[v4]), transform.TransformPoint(cubeMesh.mesh.vertices[v1]), rayHitPoint);

        //find the distances from the hit point, to the closest edges 1/2/3/4
        float distance1 = Vector3.Distance(rayHitPoint, edge1ClosestPointToRay);
        float distance2 = Vector3.Distance(rayHitPoint, edge2ClosestPointToRay);
        float distance3 = Vector3.Distance(rayHitPoint, edge3ClosestPointToRay);
        float distance4 = Vector3.Distance(rayHitPoint, edge4ClosestPointToRay);
        
        //put them in an array to sort them
        float[] distances = {distance1, distance2, distance3, distance4};
        Array.Sort(distances);

        
       // Debug.Log("distance1 " + distance1);
       // Debug.Log("distance2 " + distance2);
       // Debug.Log("distance3 " + distance3);
       // Debug.Log("distance4 " + distance4);
        

        //set the vertex 1 and 2 positions depending on which distance is closest
        //distances[0] being the closest edge
        if(distances[0] == distance1)
        {
            vertex1PosOfClosestEdge =  transform.TransformPoint(cubeMesh.mesh.vertices[v1]);
            vertex2PosOfClosestEdge =  transform.TransformPoint(cubeMesh.mesh.vertices[v2]);
        }
        if(distances[0] == distance2)
        {
            vertex1PosOfClosestEdge =  transform.TransformPoint(cubeMesh.mesh.vertices[v2]);
            vertex2PosOfClosestEdge =  transform.TransformPoint(cubeMesh.mesh.vertices[v3]);
        }
        if(distances[0] == distance3)
        {
            vertex1PosOfClosestEdge =  transform.TransformPoint(cubeMesh.mesh.vertices[v3]);
            vertex2PosOfClosestEdge =  transform.TransformPoint(cubeMesh.mesh.vertices[v4]);
        }
        if(distances[0] == distance4)
        {
            vertex1PosOfClosestEdge =  transform.TransformPoint(cubeMesh.mesh.vertices[v4]);
            vertex2PosOfClosestEdge =  transform.TransformPoint(cubeMesh.mesh.vertices[v1]);
        }

    }


    //mostly used to make sure the closest corner of the edge is picked in the cube creator indexState 0
    //very messy...
    public Vector3 GetClosestCornerFromClosestEdgeTwoVectors(Vector3 rayHitPoint, int index)
    {
        float distance1 = Vector3.Distance(rayHitPoint, vertex1PosOfClosestEdge);
        float distance2 = Vector3.Distance(rayHitPoint, vertex2PosOfClosestEdge);
        
        Vector3 closestCornerOfEdgeFromRayHit;
        Vector3 secondClosestCornerOfEdgeFromRayHit;


        if(distance1 < distance2)
        {
            closestCornerOfEdgeFromRayHit = vertex1PosOfClosestEdge;
            secondClosestCornerOfEdgeFromRayHit = vertex2PosOfClosestEdge;
        }
        else
        {
            closestCornerOfEdgeFromRayHit = vertex2PosOfClosestEdge;
            secondClosestCornerOfEdgeFromRayHit = vertex1PosOfClosestEdge;
        }

        //if index 1 = closest, index 2 = second closeset
        if(index == 1)
        {
            return closestCornerOfEdgeFromRayHit;
        }
        else
        {
            return secondClosestCornerOfEdgeFromRayHit;
        }
        
    }

public float GetSqrMagFromEdge(Vector3 vertex1, Vector3 vertex2, Vector3 rayHitPoint)
 {
    float n = Vector3.Cross(rayHitPoint - vertex1, rayHitPoint - vertex2).sqrMagnitude;
    return n / (vertex1 - vertex2).sqrMagnitude;
}


public Vector3 GetClosestPointOnEdgeFromRayHitAndTwoPositions(Vector3 vertex1, Vector3 vertex2, Vector3 rayHitPoint)
{
    return vertex1 + Vector3.Project(rayHitPoint - vertex1, vertex2 - vertex1);
}



    //used to find which corner a certain vector3 belongs to
    public SaveAndLoadLevel.Corner GetTheCornerFromExactPosition(Vector3 pos)
    {

        //top
        if(pos == transform.TransformPoint(cubeMesh.mesh.vertices[0]))
        {
            return corner_Y_Xneg_Zneg;
        }
        if(pos == transform.TransformPoint(cubeMesh.mesh.vertices[1]))
        {
            return corner_Y_Xneg_Z;
        }
        if(pos == transform.TransformPoint(cubeMesh.mesh.vertices[2]))
        {
            return corner_Y_X_Z;
        }
        if(pos == transform.TransformPoint(cubeMesh.mesh.vertices[3]))
        {
            return corner_Y_X_Zneg;
        }

        //bottom
        if(pos == transform.TransformPoint(cubeMesh.mesh.vertices[4]))
        {
            return corner_Yneg_X_Z;
        }
        if(pos == transform.TransformPoint(cubeMesh.mesh.vertices[5]))
        {
            return corner_Yneg_X_Zneg;
        }
        if(pos == transform.TransformPoint(cubeMesh.mesh.vertices[10]))
        {
            return corner_Yneg_Xneg_Z;
        }
        if(pos == transform.TransformPoint(cubeMesh.mesh.vertices[11]))
        {
            return corner_Yneg_Xneg_Zneg;
        }

        return null;

    }


    //Note: this scales the block in only the positive axis... NOT on both sides like in the unity inspector
    // it ADDS to the lenght... it DOES NOT set it...
    public void ScaleBlock_Add(Vector3 scale)
    {
        List<SaveAndLoadLevel.Corner> corners = new List<SaveAndLoadLevel.Corner>();
        corners = GetCornersFromFace_FromHitPoint(0);

        foreach(SaveAndLoadLevel.Corner c in corners)
        {
            c.corner_Pos += (transform.up * scale.y);
        }
        
        corners.Clear();
        corners = GetCornersFromFace_FromHitPoint(2);
        
        foreach(SaveAndLoadLevel.Corner c in corners)
        {
            c.corner_Pos += (transform.right * scale.x);
        }
        corners.Clear();
        corners = GetCornersFromFace_FromHitPoint(10);
        
        foreach(SaveAndLoadLevel.Corner c in corners)
        {
            c.corner_Pos += (transform.forward * scale.z);
        }

        UpdateCorners();
    }


    //USED FOR RETRIVING THE CORNERS FROM HIT POINT
    public List<SaveAndLoadLevel.Corner> GetClosestCorner_FromHitPoint(Vector3 hitPos)
    {
        CornerClosestOffsetToEditToRayHit(hitPos);
        
        List<SaveAndLoadLevel.Corner> corners = new List<SaveAndLoadLevel.Corner>();
        corners.Add(closestCornerPoint);

        return corners;
    }

    public List<SaveAndLoadLevel.Corner> GetCornersFromClosestEdge_FromHitPoint(int triangleIndex, Vector3 hitPos)
    {
        NearestEdgeFromHitPoint(triangleIndex, hitPos);
        
        List<SaveAndLoadLevel.Corner> corners = new List<SaveAndLoadLevel.Corner>();

        corners.Add(GetTheCornerFromExactPosition(vertex1PosOfClosestEdge));
        corners.Add(GetTheCornerFromExactPosition(vertex2PosOfClosestEdge));

        return corners;
    }

    public List<SaveAndLoadLevel.Corner> GetCornersFromFace_FromHitPoint(int triangleIndex)
    {        
        List<SaveAndLoadLevel.Corner> corners = new List<SaveAndLoadLevel.Corner>();

        //note... these are manually set to make the line renderer create a square

        //y
        if(triangleIndex == 0 || triangleIndex == 1)
        {
            corners.Add(corner_Y_Xneg_Zneg);
            corners.Add(corner_Y_X_Zneg);
            corners.Add(corner_Y_X_Z);
            corners.Add(corner_Y_Xneg_Z);
        }

        //x
        if(triangleIndex == 2 || triangleIndex == 3)
        {
            corners.Add(corner_Yneg_X_Z);
            corners.Add(corner_Y_X_Z);
            corners.Add(corner_Y_X_Zneg);
            corners.Add(corner_Yneg_X_Zneg);
        }

        //y neg
        if(triangleIndex == 4 || triangleIndex == 5)
        {
            corners.Add(corner_Yneg_X_Zneg);
            corners.Add(corner_Yneg_Xneg_Zneg);
            corners.Add(corner_Yneg_Xneg_Z);
            corners.Add(corner_Yneg_X_Z);        
        }

        //x neg
        if(triangleIndex == 6 || triangleIndex == 7)
        {
            corners.Add(corner_Yneg_Xneg_Zneg);
            corners.Add(corner_Y_Xneg_Zneg);
            corners.Add(corner_Y_Xneg_Z);
            corners.Add(corner_Yneg_Xneg_Z);       
        }

        //z neg
        if(triangleIndex == 8 || triangleIndex == 9)
        {
            corners.Add(corner_Yneg_X_Zneg);
            corners.Add(corner_Y_X_Zneg);
            corners.Add(corner_Y_Xneg_Zneg);
            corners.Add(corner_Yneg_Xneg_Zneg); 
        }

        //z
        if(triangleIndex == 10 || triangleIndex == 11)
        {
            corners.Add(corner_Yneg_Xneg_Z);
            corners.Add(corner_Y_Xneg_Z);
            corners.Add(corner_Y_X_Z);
            corners.Add(corner_Yneg_X_Z); 
        }



        return corners;
    }


    public void GetCornersFromTriangleIndex(int triangleIndex)
    {
       
    }




    public void UpdateCorners() //actually manipulates the mesh
    {


        //IMPORTANT, without this, older maps that dont have corner posistions saved ( = 0,0,0), would break!   Used on level load 
        if(corner_Y_X_Z.corner_Pos == Vector3.zero && SaveAndLoadLevel.Instance.fileLevelData.gameVersion == default(string))
        {
            UpdateCornerPositions();
            return;
        }


        verts = cubeMesh.mesh.vertices;

        //convert to local scape for the verticy
        // top

        
        verts[2] = transform.InverseTransformPoint(corner_Y_X_Z.corner_Pos);
        verts[7] = transform.InverseTransformPoint(corner_Y_X_Z.corner_Pos);
        verts[22] = transform.InverseTransformPoint(corner_Y_X_Z.corner_Pos);
        
        verts[3] = transform.InverseTransformPoint(corner_Y_X_Zneg.corner_Pos);
        verts[6] = transform.InverseTransformPoint(corner_Y_X_Zneg.corner_Pos);
        verts[19] = transform.InverseTransformPoint(corner_Y_X_Zneg.corner_Pos);


        verts[1] = transform.InverseTransformPoint(corner_Y_Xneg_Z.corner_Pos);
        verts[14] = transform.InverseTransformPoint(corner_Y_Xneg_Z.corner_Pos);
        verts[23] = transform.InverseTransformPoint(corner_Y_Xneg_Z.corner_Pos);
        
        verts[0] = transform.InverseTransformPoint(corner_Y_Xneg_Zneg.corner_Pos);
        verts[15] = transform.InverseTransformPoint(corner_Y_Xneg_Zneg.corner_Pos);
        verts[18] = transform.InverseTransformPoint(corner_Y_Xneg_Zneg.corner_Pos);

        //bottom
        verts[4] = transform.InverseTransformPoint(corner_Yneg_X_Z.corner_Pos);
        verts[9] = transform.InverseTransformPoint(corner_Yneg_X_Z.corner_Pos);
        verts[21] = transform.InverseTransformPoint(corner_Yneg_X_Z.corner_Pos);

        verts[5] = transform.InverseTransformPoint(corner_Yneg_X_Zneg.corner_Pos);
        verts[8] = transform.InverseTransformPoint(corner_Yneg_X_Zneg.corner_Pos);
        verts[16] = transform.InverseTransformPoint(corner_Yneg_X_Zneg.corner_Pos);

        verts[10] = transform.InverseTransformPoint(corner_Yneg_Xneg_Z.corner_Pos);
        verts[13] = transform.InverseTransformPoint(corner_Yneg_Xneg_Z.corner_Pos);
        verts[20] = transform.InverseTransformPoint(corner_Yneg_Xneg_Z.corner_Pos);

        verts[11] = transform.InverseTransformPoint(corner_Yneg_Xneg_Zneg.corner_Pos);
        verts[12] = transform.InverseTransformPoint(corner_Yneg_Xneg_Zneg.corner_Pos);
        verts[17] = transform.InverseTransformPoint(corner_Yneg_Xneg_Zneg.corner_Pos);

        cubeMesh.mesh.vertices = verts;
        cubeMesh.mesh.RecalculateBounds();
        cubeMesh.mesh.RecalculateNormals();
        UpdateMeshCollider();
    }


    public void UpdateCornerPositions() //for assigning the "finalizing" the corner positions(mostly for saving)
    {
        //top 
        corner_Y_X_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[2]);
        corner_Y_X_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[3]);
        corner_Y_Xneg_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[1]);
        corner_Y_Xneg_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[0]);

        //bottom
        corner_Yneg_X_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[4]);
        corner_Yneg_X_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[5]);
        corner_Yneg_Xneg_Z.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[10]);
        corner_Yneg_Xneg_Zneg.corner_Pos = transform.TransformPoint(cubeMesh.mesh.vertices[11]);
    }

    public Vector3 GetBlockScaleBasedLocalVertexPositions()
    {
        Vector3 scale;

        float x = Vector3.Distance(cubeMesh.mesh.vertices[5], cubeMesh.mesh.vertices[11]);
        float y = Vector3.Distance(cubeMesh.mesh.vertices[2], cubeMesh.mesh.vertices[4]);
        float z = Vector3.Distance(cubeMesh.mesh.vertices[4], cubeMesh.mesh.vertices[5]);

        scale = new Vector3(x, y, z);
        return  scale;
    }

/*
    public void ResetCornerPositions()
    {
        //position the cube inside the parent of this block
        BlockBaseCubePositionFinder_Singleton.Instance.PositionThisCubeInsideBlock(transform.parent);


        BlockFaceTextureUVProperties uvProp = BlockBaseCubePositionFinder_Singleton.Instance.GetComponent<BlockFaceTextureUVProperties>();
        //update the corner positions before getting them
        uvProp.UpdateCornerPositions();



        //top 
        corner_Y_X_Z.corner_Pos = uvProp.corner_Y_X_Z.corner_Pos;
        corner_Y_X_Zneg.corner_Pos = uvProp.corner_Y_X_Zneg.corner_Pos;
        corner_Y_Xneg_Z.corner_Pos = uvProp.corner_Y_Xneg_Z.corner_Pos;
        corner_Y_Xneg_Zneg.corner_Pos = uvProp.corner_Y_Xneg_Zneg.corner_Pos;

        //bottom
        corner_Yneg_X_Z.corner_Pos = uvProp.corner_Yneg_X_Z.corner_Pos;
        corner_Yneg_X_Zneg.corner_Pos = uvProp.corner_Yneg_X_Zneg.corner_Pos;
        corner_Yneg_Xneg_Z.corner_Pos = uvProp.corner_Yneg_Xneg_Z.corner_Pos;
        corner_Yneg_Xneg_Zneg.corner_Pos = uvProp.corner_Yneg_Xneg_Zneg.corner_Pos;

        UpdateCorners();
    }
*/

    public void UpdateCornerOffsets()
    {
        //position the cube inside the parent of this block
        BlockBaseCubePositionFinder_Singleton.Instance.PositionThisCubeInsideBlock(transform, originalPivot);


        BlockFaceTextureUVProperties uvProp = BlockBaseCubePositionFinder_Singleton.Instance.GetComponent<BlockFaceTextureUVProperties>();
        //update the corner positions before getting them
        uvProp.UpdateCornerPositions();


        //calculate the offsets from the LOCAL vertex positions...
        corner_Y_X_Z_offset = uvProp.cubeMesh.mesh.vertices[2] - cubeMesh.mesh.vertices[2] + originalPivot; 
        corner_Y_X_Zneg_offset = uvProp.cubeMesh.mesh.vertices[3] - cubeMesh.mesh.vertices[3] + originalPivot; 
        corner_Y_Xneg_Z_offset = uvProp.cubeMesh.mesh.vertices[1] - cubeMesh.mesh.vertices[1] + originalPivot; 
        corner_Y_Xneg_Zneg_offset = uvProp.cubeMesh.mesh.vertices[0] - cubeMesh.mesh.vertices[0] + originalPivot; 

        corner_Yneg_X_Z_offset = uvProp.cubeMesh.mesh.vertices[4] - cubeMesh.mesh.vertices[4] + originalPivot; 
        corner_Yneg_X_Zneg_offset = uvProp.cubeMesh.mesh.vertices[5] - cubeMesh.mesh.vertices[5] + originalPivot; 
        corner_Yneg_Xneg_Z_offset = uvProp.cubeMesh.mesh.vertices[10] - cubeMesh.mesh.vertices[10] + originalPivot; 
        corner_Yneg_Xneg_Zneg_offset = uvProp.cubeMesh.mesh.vertices[11] - cubeMesh.mesh.vertices[11] + originalPivot; 

    }





    void UpdateMeshCollider()
    {
       // GetComponent<MeshCollider>().sharedMesh = null; //this was commented out on 1/4/2025, if anything breaks check here!
        GetComponent<MeshCollider>().sharedMesh = cubeMesh.mesh;
    }


    public void SetMaterialColorBasedOnIndex(int index, Color color)
    {
        CheckIfPropertyBlocksExist();
        if(_renderer == null)
        {
        _renderer = GetComponent<Renderer>();
        }

        if(index == 0)
        {
        _renderer.GetPropertyBlock(propBlock_y, 0);
        propBlock_y.SetColor("_Color",color);
        _renderer.SetPropertyBlock(propBlock_y, 0);
        materialColor_y = color;
        }

        if(index == 1)
        {
        _renderer.GetPropertyBlock(propBlock_x, 1);
        propBlock_x.SetColor("_Color",color);
        _renderer.SetPropertyBlock(propBlock_x, 1);
        materialColor_x = color;
        }

        if(index == 2)
        {
        _renderer.GetPropertyBlock(propBlock_yneg, 2);
        propBlock_yneg.SetColor("_Color",color);
        _renderer.SetPropertyBlock(propBlock_yneg, 2);
        materialColor_yneg = color;
        }

        if(index == 3)
        {
        _renderer.GetPropertyBlock(propBlock_xneg, 3);
        propBlock_xneg.SetColor("_Color",color);
        _renderer.SetPropertyBlock(propBlock_xneg, 3);
        materialColor_xneg = color;
        }

        if(index == 4)
        {
        _renderer.GetPropertyBlock(propBlock_zneg, 4);
        propBlock_zneg.SetColor("_Color",color);
        _renderer.SetPropertyBlock(propBlock_zneg, 4);
        materialColor_zneg = color;
        }

        if(index == 5)
        {
        _renderer.GetPropertyBlock(propBlock_z, 5);
        propBlock_z.SetColor("_Color",color);
        _renderer.SetPropertyBlock(propBlock_z, 5);
        materialColor_z = color;
        }

        UpdateBlockUV();
    }

    public Color GetMaterialColorBasedOnIndex(int index)
    {
        if(index == 0)
        {
            return materialColor_y;
        }
        if(index == 1)
        {
            return materialColor_x;
        }
        if(index == 2)
        {
            return materialColor_yneg;
        }
        if(index == 3)
        {
            return materialColor_xneg;
        }
        if(index == 4)
        {
            return materialColor_zneg;
        }
        if(index == 5)
        {
            return materialColor_z;
        }
        return materialColor_y;
    }

    // Update is called once per frame
    public void UpdateBlockUV()
    {
        UpdateCornerOffsets(); //call this because the bottom code relies on corner offsets.


        if(parent != null)
        {

            //Quaterion (this is what applies the rotation)  *  new Vector2 (this is what applies to offset and scale)
           Vector2[] cubeuv = new Vector2[24]
        {


            //HOW THE HELL ARE THE CORNER OFFSETS BEING APPLIED???
            //Note: Only the "side ways" axis of each face is applied. NOT the height, since uv's are vector 2. (To try it out in any 3d editor, move the vertex in the "depth" axis...)
            //Its simple... the UVS are in the same order as the vertexes... Just follow the order and apply the appropriate corner that modifies that corner


                    // Rotation * (Offset and scales + offsets for proper vertex editing
            //top                                                                                                                                                                              //vertex editing offsets                 //unfortanately... you have to multiple the apprppriate scale and uv scale too...
            Quaternion.Euler(0,0,UVRotation[0]) * (new Vector2(0 + UVOffSet[0].x, 0 + UVOffSet[0].y)                                                                                        +new Vector2(-corner_Y_Xneg_Zneg_offset.x * UVScale[0].x,  -corner_Y_Xneg_Zneg_offset.z * UVScale[0].y)),
            Quaternion.Euler(0,0,UVRotation[0]) * (new Vector2(0 + UVOffSet[0].x, UVScale[0].y + UVOffSet[0].y)                                             +new Vector2(-corner_Y_Xneg_Z_offset.x * UVScale[0].x,  -corner_Y_Xneg_Z_offset.z * UVScale[0].y)),
            Quaternion.Euler(0,0,UVRotation[0]) * (new Vector2( UVScale[0].x + UVOffSet[0].x,UVScale[0].y + UVOffSet[0].y)  +new Vector2(-corner_Y_X_Z_offset.x * UVScale[0].x,  -corner_Y_X_Z_offset.z * UVScale[0].y)),
            Quaternion.Euler(0,0,UVRotation[0]) * (new Vector2( UVScale[0].x + UVOffSet[0].x, 0 + UVOffSet[0].y)                                             +new Vector2(-corner_Y_X_Zneg_offset.x * UVScale[0].x,  -corner_Y_X_Zneg_offset.z * UVScale[0].y)),

            // x+ side
            Quaternion.Euler(0,0,UVRotation[1]) * (new Vector2(UVScale[1].x + UVOffSet[1].x, 0 + UVOffSet[1].y)                                              +new Vector2(-corner_Yneg_X_Z_offset.z * UVScale[1].x,  -corner_Yneg_X_Z_offset.y * UVScale[1].y)), //use z and y because the "y" is the vertical axis for a cube with a 0,0,0 rotation                                            
            Quaternion.Euler(0,0,UVRotation[1]) * (new Vector2(0 + UVOffSet[1].x, 0 + UVOffSet[1].y)                                                                                         +new Vector2(-corner_Yneg_X_Zneg_offset.z * UVScale[1].x,  -corner_Yneg_X_Zneg_offset.y * UVScale[1].y)),
            Quaternion.Euler(0,0,UVRotation[1]) * (new Vector2(0 + UVOffSet[1].x, UVScale[1].y + UVOffSet[1].y)                                              +new Vector2(-corner_Y_X_Zneg_offset.z * UVScale[1].x,  -corner_Y_X_Zneg_offset.y * UVScale[1].y)),
            Quaternion.Euler(0,0,UVRotation[1]) * (new Vector2(UVScale[1].x + UVOffSet[1].x, UVScale[1].y + UVOffSet[1].y)   +new Vector2(-corner_Y_X_Z_offset.z * UVScale[1].x,  -corner_Y_X_Z_offset.y * UVScale[1].y)),

            // bottom                                                                                                                                                                         //for some reason... you need to use +x here instead of -x ...
            Quaternion.Euler(0,0,UVRotation[2]) * (new Vector2(0 + UVOffSet[2].x, 0 + UVOffSet[2].y)                                                                                         +new Vector2(corner_Yneg_X_Zneg_offset.x * UVScale[2].x,  -corner_Yneg_X_Zneg_offset.z * UVScale[2].y)),
            Quaternion.Euler(0,0,UVRotation[2]) * (new Vector2(0 + UVOffSet[2].x, UVScale[2].y + UVOffSet[2].y)                                              +new Vector2(corner_Yneg_X_Z_offset.x * UVScale[2].x,  -corner_Yneg_X_Z_offset.z * UVScale[2].y)),
            Quaternion.Euler(0,0,UVRotation[2]) * (new Vector2(UVScale[2].x + UVOffSet[2].x, UVScale[2].y + UVOffSet[2].y)   +new Vector2(corner_Yneg_Xneg_Z_offset.x * UVScale[2].x,  -corner_Yneg_Xneg_Z_offset.z * UVScale[2].y)),
            Quaternion.Euler(0,0,UVRotation[2]) * (new Vector2(UVScale[2].x + UVOffSet[2].x, 0 + UVOffSet[2].y)                                              +new Vector2(corner_Yneg_Xneg_Zneg_offset.x * UVScale[2].x,  -corner_Yneg_Xneg_Zneg_offset.z * UVScale[2].y)),

            //x- side
            Quaternion.Euler(0,0,UVRotation[3]) * (new Vector2(UVScale[3].x + UVOffSet[3].x, 0 + UVOffSet[3].y)                                              +new Vector2(corner_Yneg_Xneg_Zneg_offset.z *  UVScale[3].x,  -corner_Yneg_Xneg_Zneg_offset.y * UVScale[3].y)),
            Quaternion.Euler(0,0,UVRotation[3]) * (new Vector2(0 + UVOffSet[3].x, 0 + UVOffSet[3].y)                                                                                         +new Vector2(corner_Yneg_Xneg_Z_offset.z * UVScale[3].x,  -corner_Yneg_Xneg_Z_offset.y *  UVScale[3].y)),
            Quaternion.Euler(0,0,UVRotation[3]) * (new Vector2(0 + UVOffSet[3].x, UVScale[3].y + UVOffSet[3].y)                                              +new Vector2(corner_Y_Xneg_Z_offset.z *  UVScale[3].x,  -corner_Y_Xneg_Z_offset.y *  UVScale[3].y)),
            Quaternion.Euler(0,0,UVRotation[3]) * (new Vector2(UVScale[3].x + UVOffSet[3].x, UVScale[3].y + UVOffSet[3].y)   +new Vector2(corner_Y_Xneg_Zneg_offset.z * UVScale[3].x,  -corner_Y_Xneg_Zneg_offset.y * UVScale[3].y)),

            //z- side
            Quaternion.Euler(0,0,UVRotation[4]) * (new Vector2(UVScale[4].x + UVOffSet[4].x, 0 + UVOffSet[4].y)                                              +new Vector2(-corner_Yneg_X_Zneg_offset.x * UVScale[4].x,  -corner_Yneg_X_Zneg_offset.y * UVScale[4].y)),
            Quaternion.Euler(0,0,UVRotation[4]) * (new Vector2(0 + UVOffSet[4].x, 0 + UVOffSet[4].y)                                                                                         +new Vector2(-corner_Yneg_Xneg_Zneg_offset.x  * UVScale[4].x,  -corner_Yneg_Xneg_Zneg_offset.y * UVScale[4].y)),
            Quaternion.Euler(0,0,UVRotation[4]) * (new Vector2(0 + UVOffSet[4].x, UVScale[4].y + UVOffSet[4].y)                                              +new Vector2(-corner_Y_Xneg_Zneg_offset.x  * UVScale[4].x,  -corner_Y_Xneg_Zneg_offset.y * UVScale[4].y)),
            Quaternion.Euler(0,0,UVRotation[4]) * (new Vector2(UVScale[4].x + UVOffSet[4].x, UVScale[4].y + UVOffSet[4].y)   +new Vector2(-corner_Y_X_Zneg_offset.x  * UVScale[4].x,  -corner_Y_X_Zneg_offset.y * UVScale[4].y)),
            
            //z+ side
            Quaternion.Euler(0,0,UVRotation[5]) * (new Vector2(UVScale[5].x + UVOffSet[5].x    , 0 + UVOffSet[5].y)                                          +new Vector2(corner_Yneg_Xneg_Z_offset.x * UVScale[5].x,  -corner_Yneg_Xneg_Z_offset.y * UVScale[5].y)),
            Quaternion.Euler(0,0,UVRotation[5]) * (new Vector2(0 + UVOffSet[5].x  , 0 + UVOffSet[5].y)                                                                                       +new Vector2(corner_Yneg_X_Z_offset.x * UVScale[5].x,  -corner_Yneg_X_Z_offset.y * UVScale[5].y)),
            Quaternion.Euler(0,0,UVRotation[5]) * (new Vector2(0 + UVOffSet[5].x , (UVScale[5].y + UVOffSet[5].y))                                           +new Vector2(corner_Y_X_Z_offset.x * UVScale[5].x,  -corner_Y_X_Z_offset.y * UVScale[5].y)),
            Quaternion.Euler(0,0,UVRotation[5]) * (new Vector2(UVScale[5].x + UVOffSet[5].x, UVScale[5].y + UVOffSet[5].y)   +new Vector2(corner_Y_Xneg_Z_offset.x * UVScale[5].x,  -corner_Y_Xneg_Z_offset.y * UVScale[5].y))
        };

        

       SetLayerBasedOnBlockMaterial(); //call this here, so that you dont have to call this in all other classes.
    

       cubeMesh.mesh.uv = cubeuv; 


        } 
    } 
    
    public void ResetFaceScale(int index)
    {
        UVScale[index] = new Vector2(1,1);
        UpdateBlockUV();
    }

    public void ResetFaceOffset(int index)
    {
        UVOffSet[index] = new Vector2(0,0);
        UpdateBlockUV();
    }

    public void ResetFaceRotation(int index)
    {
        UVRotation[index] = 0f;
        UpdateBlockUV();
    }

    public string GetMatNameBasedOnIndex(int index)
    {
        Material[] matArray = _renderer.sharedMaterials;

        if(matArray[index] != null)
        {
        return matArray[index].name;
        }
        else
        {
        return "";
        }

    }

    public Material GetMaterialBasedOnIndex(int index)
    {
        Material[] matArray = GetComponent<GeneralObjectInfo>().baseMatList;
        return matArray[index];
    }

    public void ChangeFaceMatName(int index, string name)
    {
        if(index == 0)
        {
            materialName_y = name;
        }
        if(index == 1)
        {
            materialName_x = name;
        }
        if(index == 2)
        {
            materialName_yneg = name;
        }
        if(index == 3)
        {
            materialName_xneg = name;
        }
        if(index == 4)
        {
            materialName_zneg = name;
        }
        if(index == 5)
        {
            materialName_z = name;
        }

    }

    public void ChangeFaceMat(int faceIndex, Material mat)
    {
        Material[] matArray = _renderer.sharedMaterials;
        matArray[faceIndex] = mat;
        _renderer.sharedMaterials = matArray;

        GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();
    }


    //for painting the entire block
    public void PaintEntireBlock_Material(Material mat)
    {
        ChangeFaceMatName(0, mat.name);
        ChangeFaceMatName(1, mat.name);
        ChangeFaceMatName(2, mat.name);
        ChangeFaceMatName(3, mat.name);
        ChangeFaceMatName(4, mat.name);
        ChangeFaceMatName(5, mat.name);

        ChangeFaceMat(0, mat);
        ChangeFaceMat(1, mat);
        ChangeFaceMat(2, mat);
        ChangeFaceMat(3, mat);
        ChangeFaceMat(4, mat);
        ChangeFaceMat(5, mat);
    }
    public void PaintEntireBlock_Color(Color color)
    {
        SetMaterialColorBasedOnIndex(0, color);
        SetMaterialColorBasedOnIndex(1, color);
        SetMaterialColorBasedOnIndex(2, color);
        SetMaterialColorBasedOnIndex(3, color);
        SetMaterialColorBasedOnIndex(4, color);
        SetMaterialColorBasedOnIndex(5, color);
    }


    public void PaintEntireBlock_SetUV(Vector2 offset, Vector2 scale, float rotation)
    {
        UVOffSet[0] = offset;
        UVOffSet[1] = offset;
        UVOffSet[2] = offset;
        UVOffSet[3] = offset;
        UVOffSet[4] = offset;
        UVOffSet[5] = offset;

        UVScale[0] = scale;
        UVScale[1] = scale;
        UVScale[2] = scale;
        UVScale[3] = scale;
        UVScale[4] = scale;
        UVScale[5] = scale;

        UVRotation[0] = rotation;
        UVRotation[1] = rotation;
        UVRotation[2] = rotation;
        UVRotation[3] = rotation;
        UVRotation[4] = rotation;
        UVRotation[5] = rotation;
    }




    //this usually runs at the end of the level load steps... used for shared materials, (that come from posters)
    public void AssignSharedMaterialBasedOnGUID()
    {
        if(_renderer == null)
        {
        _renderer = GetComponent<Renderer>();
        }

        Material[] matArray = _renderer.sharedMaterials;

        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster != null)
            {
                if(materialName_y != null && poster.name == materialName_y)
                {
                    matArray[0] = poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial;
                }
                if(materialName_x != null && poster.name == materialName_x)
                {
                    matArray[1] = poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial;
                }
                if(materialName_yneg != null && poster.name == materialName_yneg)
                {
                    matArray[2] = poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial;
                }
                if(materialName_xneg != null && poster.name == materialName_xneg)
                {
                    matArray[3] = poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial;
                }
                if(materialName_zneg != null && poster.name == materialName_zneg)
                {
                    matArray[4] = poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial;
                }
                if(materialName_z != null && poster.name == materialName_z)
                {
                    matArray[5] = poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial;
                }
            }
        }

        _renderer.sharedMaterials = matArray;

        if(GetComponent<GeneralObjectInfo>())
        {
            GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();
        }
    }


    public bool CheckIfFaceHasXrayPropertiesBasedOnIndex(int triangleIndex)
    {
        Material[] matArray = _renderer.sharedMaterials;

        //y
        if(triangleIndex == 0 || triangleIndex == 1)
        {
            return CheckIfFaceContainsXraySettings(matArray[0]);
        }

        //x
        if(triangleIndex == 2 || triangleIndex == 3)
        {
            return CheckIfFaceContainsXraySettings(matArray[1]);
        }

        //y neg
        if(triangleIndex == 4 || triangleIndex == 5)
        {
            return CheckIfFaceContainsXraySettings(matArray[2]);
        }

        //x neg
        if(triangleIndex == 6 || triangleIndex == 7)
        {
            return CheckIfFaceContainsXraySettings(matArray[3]);
        }

        //z neg
        if(triangleIndex == 8 || triangleIndex == 9)
        {
            return CheckIfFaceContainsXraySettings(matArray[4]);
        }

        //z
        if(triangleIndex == 10 || triangleIndex == 11)
        {
            return CheckIfFaceContainsXraySettings(matArray[5]);
        }

        return false;
    }

    public bool CheckIfFaceContainsXraySettings(Material mat)
    {
        if(mat.GetFloat("_size") > 0 )
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    public bool BlockContainsLightenMaterial()
    {
        Material[] matArray = _renderer.sharedMaterials;

        foreach(Material mat in matArray)
        {
            if(mat)
            {
                if(mat.shader.name == "Custom/FakeLighting_Lighten")
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool BlockContainsDarkenMaterial()
    {
        Material[] matArray = _renderer.sharedMaterials;

        foreach(Material mat in matArray)
        {
            if(mat)
            {
                if(mat.shader.name == "Custom/FakeLighting_Darken")
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetLayerBasedOnBlockMaterial()
    {
        if(BlockContainsLightenMaterial() || BlockContainsDarkenMaterial())
        {
            gameObject.layer = 20;
        }
        else
        {
            gameObject.GetComponent<GeneralObjectInfo>().UpdateGeneralObjectLayerProperties();
        }
    }



    public Vector3 CalculateWorldPosCenterFaceFromFourIndexes(int ind1, int ind2, int ind3, int ind4)
    {
                    Vector3 vertex0 = cubeMesh.mesh.vertices[ind1];
                    Vector3 vertexWorldPos0 = transform.TransformPoint(vertex0);

                    Vector3 vertex1 = cubeMesh.mesh.vertices[ind2];
                    Vector3 vertexWorldPos1 = transform.TransformPoint(vertex1);

                    Vector3 vertex2 = cubeMesh.mesh.vertices[ind3];
                    Vector3 vertexWorldPos2 = transform.TransformPoint(vertex2);
                    
                    Vector3 vertex3 = cubeMesh.mesh.vertices[ind4];
                    Vector3 vertexWorldPos3 = transform.TransformPoint(vertex3);

                    List<Vector3> topIndexes = new List<Vector3>();
                    topIndexes.Add(vertexWorldPos0);
                    topIndexes.Add(vertexWorldPos1);
                    topIndexes.Add(vertexWorldPos2);
                    topIndexes.Add(vertexWorldPos3);

        return GlobalUtilityFunctions.CalculateAverageVectorPositionFromListOfVector3(topIndexes);
    }

    public Vector3 GetFaceCenterPos_Top()
    {
        return CalculateWorldPosCenterFaceFromFourIndexes(0,1,2,3);
    }
    public Vector3 GetFaceCenterPos_Bot()
    {
        return CalculateWorldPosCenterFaceFromFourIndexes(8,9,10,11);
    }
    public Vector3 GetFaceCenterPos_X_Front()
    {
        return CalculateWorldPosCenterFaceFromFourIndexes(4,5,6,7);
    }
    public Vector3 GetFaceCenterPos_X_Back()
    {
        return CalculateWorldPosCenterFaceFromFourIndexes(12,13,14,15);
    }
    public Vector3 GetFaceCenterPos_Z_Front()
    {
        return CalculateWorldPosCenterFaceFromFourIndexes(20,21,22,23);

    }
    public Vector3 GetFaceCenterPos_Z_Back()
    {
        return CalculateWorldPosCenterFaceFromFourIndexes(16,17,18,19);
    }


   //WARNING, uncommenting this will cause a problem where the prefab mesh keeps changing to "cube" instead of "defaultcube"

   /*
            void OnDrawGizmosSelected()
    {

        for(int i2 = 0; i2 < cubeMesh.mesh.uv.Length; i2++)
        {
//            Debug.Log("uv " + i2 + ": " + Mathf.RoundToInt(cubeMesh.mesh.uv[i2].x) + " " + Mathf.RoundToInt(cubeMesh.mesh.uv[i2].y));
        }


        float sumSpacer = 0.0f;
              //draw main frame quad
         int i = 0;
                 foreach (Vector3 vertex in cubeMesh.mesh.vertices) {
        Vector3 vertexWorldPos = gameObject.transform.TransformPoint(vertex);

        Handles.Label(vertexWorldPos + new Vector3(sumSpacer,sumSpacer,sumSpacer), i.ToString());
        i++;
        sumSpacer+=0.002f;
          }  
    
    }
  */


    //PIVOT EDITING MECHANIC

    //Note: the way it works: 1. save corner positions, 2. set pivot to center of the saved corner list positions. 3. set the block corner positions to the first saved list(from step 1) 
    //Note: the children get unparented, and then parented again... this is so that the children dont move then the pivot(parent transform) is moved

    public void SetPivotToBlockCenter()
    {
        Vector3 startWorldPos = transform.position;


        //SAVE all children, and unparent
        List<Transform> children = new List<Transform>();
        foreach(Transform child in transform)
        {
            if(child.GetComponent<GeneralObjectInfo>())
            {
                child.SetParent(null);
                children.Add(child);
            }
        }




        //1. save the current corner position list
        SaveAndLoadLevel.Corner[] corners = GetCurrentListOfUpdatedCorners();

        //2. calculate the average center point of the list, and set the object(pivot) to that position
        List<Vector3> cornerPositions = new List<Vector3>();
        foreach(SaveAndLoadLevel.Corner c in corners)
        {
            cornerPositions.Add(c.corner_Pos);
        }

        Vector3 averageCenterPost = GlobalUtilityFunctions.CalculateAverageVectorPositionFromListOfVector3(cornerPositions);
        transform.position = averageCenterPost;
        GetComponent<GeneralObjectInfo>().UpdatePosition(); //update position or else for some reason there will be errors when adjusting the corner positions(was happening when adjusting corners -> saving map)

        //convert the stored world pos(before the pivot was moved) to local space of the cube. then add this offset to "originalPivot"
        originalPivot += transform.InverseTransformPoint(startWorldPos);

        //3. set the corner positions back
        SetCornersFromCornerList(corners);
    




        //SET the children parenting back...
        foreach(Transform child in children)
        {
            child.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
        }
    
    }



    //Mostly used only when a block is created (this offsets the pivot so that the texturing aligns to the bottom left of the dev texture)
    public void ApplyCubeSpawnPivotOffset()
    {
        float width = Vector3.Distance(cubeMesh.mesh.vertices[2], cubeMesh.mesh.vertices[3]);
        float height =  Vector3.Distance(cubeMesh.mesh.vertices[2], cubeMesh.mesh.vertices[4]);
        float depth = Vector3.Distance(cubeMesh.mesh.vertices[2], cubeMesh.mesh.vertices[1]);

                                    //switched!?!
        originalPivot -= new Vector3(depth/2,height/2,width/2) + new Vector3(0.5f, 0.5f, 0.5f);
    }





    //for footstep sounds
    public AudioClip GetAudioClipFromFaceBasedOnTriangleIndex(int triangleIndex)
    {        

        //y
        if(triangleIndex == 0 || triangleIndex == 1)
        {
            return GetPosterFootstepAudioClipFromPosterID(materialName_y);
        }

        //x
        if(triangleIndex == 2 || triangleIndex == 3)
        {
            return GetPosterFootstepAudioClipFromPosterID(materialName_x);
        }

        //y neg
        if(triangleIndex == 4 || triangleIndex == 5)
        {
            return GetPosterFootstepAudioClipFromPosterID(materialName_yneg);
        }

        //x neg
        if(triangleIndex == 6 || triangleIndex == 7)
        {
            return GetPosterFootstepAudioClipFromPosterID(materialName_xneg);
        }

        //z neg
        if(triangleIndex == 8 || triangleIndex == 9)
        {
            return GetPosterFootstepAudioClipFromPosterID(materialName_zneg);
        }

        //z
        if(triangleIndex == 10 || triangleIndex == 11)
        {
            return GetPosterFootstepAudioClipFromPosterID(materialName_z);
        }

        return null;
    }


    AudioClip GetPosterFootstepAudioClipFromPosterID(string posterID)
    {
        
        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster != null)
            {
                if(poster.name == posterID)
                {
                    return poster.GetComponent<PosterFootstepSound>().footStepAudioClip;
                }
            }
        }

        return null;

    }



    public float GetBlockVolume()
    {
        float width = Vector3.Distance(cubeMesh.mesh.vertices[2], cubeMesh.mesh.vertices[3]);
        float height =  Vector3.Distance(cubeMesh.mesh.vertices[2], cubeMesh.mesh.vertices[4]);
        float depth = Vector3.Distance(cubeMesh.mesh.vertices[2], cubeMesh.mesh.vertices[1]);

        return width * height * depth;
    }

    void OnDestroy()
    {
        if(SaveAndLoadLevel.Instance.allLoadedBlocks.Contains(gameObject))
        SaveAndLoadLevel.Instance.allLoadedBlocks.Remove(gameObject);
    }


}
