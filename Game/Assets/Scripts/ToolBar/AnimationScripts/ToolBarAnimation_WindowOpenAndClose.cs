using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToolBarAnimation_WindowOpenAndClose : MonoBehaviour
{
    public AudioClip open_AudioClip;
    public AudioClip close_AudioClip;
    public void OpenWindow()
    {
        if(gameObject.activeSelf == false)
        {
            ToolBarStatic_UISFXAudioSource.Instance.ToolBar_UISFX_Play(open_AudioClip);
            transform.localScale = new Vector3(1f, 0f, 1f);
            gameObject.SetActive(true);
            transform.DOScaleY(1.0f, 0.2f);
        }
    }
    public void CloseWindow()
    {
        if(gameObject.activeSelf == true)
        {
        ToolBarStatic_UISFXAudioSource.Instance.ToolBar_UISFX_Play(close_AudioClip);
        transform.DOScaleY(0.0f, 0.2f).OnComplete(() => gameObject.SetActive(false) );
        }
    }
}
