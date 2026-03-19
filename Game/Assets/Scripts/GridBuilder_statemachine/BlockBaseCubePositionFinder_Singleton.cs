using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBaseCubePositionFinder_Singleton : MonoBehaviour
{
    //used for positioning a basic cube inside a block parent at positions (0.5,0.5,0.5) to imitate an unmofified block
    //use this for getting the "reset" positions and the offsets



    private static BlockBaseCubePositionFinder_Singleton _instance;
    public static BlockBaseCubePositionFinder_Singleton Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public void PositionThisCubeInsideBlock(Transform parent, Vector3 originalPivot)
    {
        transform.parent = parent;
        transform.localPosition = originalPivot;
        transform.localScale = new Vector3(1f,1f,1f);
        transform.localRotation = Quaternion.identity;
    }

    //used for making sure this class/object does not get deleted when a block is deleted/destroyed
    public void RemoveFromCube()
    {
        transform.parent = null;
    }


}
