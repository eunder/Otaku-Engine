using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public Material lineMaterial;
    public Vector3 startPos;
    public Vector3 endPos;
    
    public GridSizeManager gridSizeManager;
    public int lineCountOnEachDirection = 100;
    
    public Color CenterAxisColor;
    public Color CenterAxisOuterColor;
    public Color GridColor;
    public float gridAlpha_center = 1.0f;
    public float gridAlpha = 1.0f;

    void Start()
    {
                if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
  void OnRenderObject()
    {
             if(Camera.current.name=="MainPlayerCam") {

        //MAKES THE GRID FIT 'NICELY' WITHOUT EXTRA LOOSE LINES ON THE SIDES
        if(gridSizeManager.gridSize == 16)
        {
            lineCountOnEachDirection = 1;
        }
        if(gridSizeManager.gridSize == 8)
        {
            lineCountOnEachDirection = 2;
        }
        if(gridSizeManager.gridSize == 4)
        {
            lineCountOnEachDirection = 5;
        }
        if(gridSizeManager.gridSize == 2)
        {
            lineCountOnEachDirection = 10;
        }
        if(gridSizeManager.gridSize == 1)
        {
            lineCountOnEachDirection = 24;
        }



       // gridAlpha = 1.0f;
          // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);

            GL.Color(new Color(CenterAxisColor.r, CenterAxisColor.g, CenterAxisColor.b, gridAlpha_center));

            //right X
            GL.Vertex(startPos + new Vector3(0,0.0f, lineCountOnEachDirection));
            GL.Vertex(startPos + new Vector3(0,0.0f, -lineCountOnEachDirection));

            //left X
            GL.Vertex(startPos + new Vector3(0,0.0f, lineCountOnEachDirection));
            GL.Vertex(startPos + new Vector3(0,0.0f, -lineCountOnEachDirection));
            
            //foreward Z 
            GL.Vertex(startPos + new Vector3(lineCountOnEachDirection,0.0f, 0));
            GL.Vertex(startPos + new Vector3(-lineCountOnEachDirection,0.0f, 0));

            //back Z
            GL.Vertex(startPos + new Vector3(lineCountOnEachDirection,0.0f, 0));
            GL.Vertex(startPos + new Vector3(-lineCountOnEachDirection,0.0f, 0));


            GL.Color(CenterAxisOuterColor);


            //add thickness to center

            //right X
            GL.Vertex(startPos + new Vector3(0.005f,0.005f, lineCountOnEachDirection));
            GL.Vertex(startPos + new Vector3(0.005f,0.005f, -lineCountOnEachDirection));

            //left X
            GL.Vertex(startPos + new Vector3(0.005f,0.005f, lineCountOnEachDirection));
            GL.Vertex(startPos + new Vector3(0.005f,0.005f, -lineCountOnEachDirection));
            
            //foreward Z 
            GL.Vertex(startPos + new Vector3(lineCountOnEachDirection,0.005f, 0.005f));
            GL.Vertex(startPos + new Vector3(-lineCountOnEachDirection,0.005f, 0.005f));

            //back Z
            GL.Vertex(startPos + new Vector3(lineCountOnEachDirection,0.005f, 0.005f));
            GL.Vertex(startPos + new Vector3(-lineCountOnEachDirection,0.005f, 0.005f));

            //right X
            GL.Vertex(startPos + new Vector3(-0.005f,-0.005f, lineCountOnEachDirection));
            GL.Vertex(startPos + new Vector3(-0.005f,-0.005f, -lineCountOnEachDirection));

            //left X
            GL.Vertex(startPos + new Vector3(-0.005f,-0.005f, lineCountOnEachDirection));
            GL.Vertex(startPos + new Vector3(-0.005f,-0.005f, -lineCountOnEachDirection));
            
            //foreward Z 
            GL.Vertex(startPos + new Vector3(lineCountOnEachDirection,-0.005f, -0.005f));
            GL.Vertex(startPos + new Vector3(-lineCountOnEachDirection,-0.005f, -0.005f));

            //back Z
            GL.Vertex(startPos + new Vector3(lineCountOnEachDirection,-0.005f, -0.005f));
            GL.Vertex(startPos + new Vector3(-lineCountOnEachDirection,-0.005f, -0.005f));


/*
        // BIG SQUARES(1 whole unit)
        for (int i = 0; i < lineCountOnEachDirection; ++i)
        {
            float a = i / (float)lineCountOnEachDirection;
            // Vertex colors change from red to green
            GL.Color(new Color(1, 1, 1,  1 - a));

            //right X
            GL.Vertex(startPos + new Vector3(i,0.0f, lineCountOnEachDirection));
            GL.Vertex(startPos + new Vector3(i,0.0f, -lineCountOnEachDirection));

            //left X
            GL.Vertex(startPos + new Vector3(-i,0.0f, lineCountOnEachDirection));
            GL.Vertex(startPos + new Vector3(-i,0.0f, -lineCountOnEachDirection));
            
            //foreward Z 
            GL.Vertex(startPos + new Vector3(lineCountOnEachDirection,0.0f, i));
            GL.Vertex(startPos + new Vector3(-lineCountOnEachDirection,0.0f, i));

            //back Z
            GL.Vertex(startPos + new Vector3(lineCountOnEachDirection,0.0f, -i));
            GL.Vertex(startPos + new Vector3(-lineCountOnEachDirection,0.0f, -i));
        }
*/

            //SMALLER SQUARES (half a unity?)
        for (int i = 1; i < lineCountOnEachDirection * gridSizeManager.gridSize; ++i)
        {
            float iPos = i/gridSizeManager.gridSize;

            GL.Color(new Color(GridColor.r, GridColor.g, GridColor.b, gridAlpha));


            GL.Vertex(startPos + new Vector3(iPos,0.0f, lineCountOnEachDirection));
            GL.Vertex(startPos + new Vector3(iPos,0.0f, -lineCountOnEachDirection));

            //left X
            GL.Vertex(startPos + new Vector3(-iPos,0.0f, lineCountOnEachDirection));
            GL.Vertex(startPos + new Vector3(-iPos,0.0f, -lineCountOnEachDirection));
            
            //foreward Z 
            GL.Vertex(startPos + new Vector3(lineCountOnEachDirection,0.0f, iPos));
            GL.Vertex(startPos + new Vector3(-lineCountOnEachDirection,0.0f, iPos));

            //back Z
            GL.Vertex(startPos + new Vector3(lineCountOnEachDirection,0.0f, -iPos));
            GL.Vertex(startPos + new Vector3(-lineCountOnEachDirection,0.0f, -iPos));

           // gridAlpha = gridAlpha - 0.05f;
           // if(0 > gridAlpha)
           // {
           //     gridAlpha = 0;
           // }
        }

        GL.End();
        GL.PopMatrix();
    }

    }
}
