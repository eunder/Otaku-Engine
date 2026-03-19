using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToolBarAnimation_SubWindow : MonoBehaviour
{

    public void OpenSubWindow()
    {
        transform.DOScaleY(1.0f, 0.16f);
    }
    public void CloseSubWindow()
    {
        transform.DOScaleY(0.0f, 0.16f);
    }

}
