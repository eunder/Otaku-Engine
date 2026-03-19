using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitRoomEvent : MonoBehaviour
{

    public RawImage blackFadeImage;
    public void exitRoomEvent()
    {
    GetComponent<AudioSource>().Play();
    blackFadeImage.DOColor(Color.black, 0.95f).OnComplete(() => ChangeScenes());
    }

    public void ChangeScenes()
    {
        SceneManager.LoadScene("PlayerRoom", LoadSceneMode.Single);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }

}
