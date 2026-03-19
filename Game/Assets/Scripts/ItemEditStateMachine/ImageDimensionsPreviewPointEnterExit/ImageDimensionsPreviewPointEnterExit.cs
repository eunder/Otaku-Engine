using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDimensionsPreviewPointEnterExit : MonoBehaviour
{
    public GameObject posterAreaPreviewObject;

    public void ShowPosterAreaPreview()
    {
        posterAreaPreviewObject.SetActive(true);;
    }
    public void HidePosterAreaPreview()
    {
        posterAreaPreviewObject.SetActive(false);;
    }
}
