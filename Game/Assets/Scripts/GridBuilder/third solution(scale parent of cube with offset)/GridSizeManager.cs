using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridSizeManager : MonoBehaviour
{
    private static GridSizeManager _instance;
    public static GridSizeManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public GameObject gridSize_Canvas;
    public TextMeshProUGUI gridSize_GUI_text;

     public float[] gridIncrements = {1,2,4,8,16,64};

    public float gridSize = 1.0f; // 4.0f = quarter blocks, 2.0 = half blocks, 1.0 = normal, 0.5 = 2 blocks

    void Update()
    {
        gridSize_GUI_text.text = "Grid Size: " + (int)gridSize;
    }

}
