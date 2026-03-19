using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCornerObject : MonoBehaviour
{
    //used to position the selected corners... an object with this class gets built whenever corners are selected

   // public BlockFaceTextureUVProperties prop;
    public SaveAndLoadLevel.Corner corner;

    float size;


    // Update is called once per frame
    void Update()
    {
        //set position of corner
        corner.corner_Pos = transform.position;

        //dynamically resize spheres like gizmos
        size = ((PlayerObjectInteractionStateMachine.Instance.playerCamera.transform.position - transform.position).magnitude) * 0.15f;
        transform.localScale = new Vector3(size,size,size);

    }
}
