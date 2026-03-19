using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MissionStatusManager : MonoBehaviour
{
    private static MissionStatusManager _instance;
    public static MissionStatusManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


   // public enum MissionState {playing, complete, gameover};
   // public MissionState currentMissionState;

    public bool missionEndEvent = false; //used to make sure either complete/gameover only happens once


    public AudioMixer audioMixer;

    public void MissionComplete()
    {
        if(missionEndEvent == false)
        {
            ScreenShake2.Instance.Shake(0.4f);
            missionEndEvent = true;

            //if the finished map is in streaming assests campaign folder... change modify the game part to "finished"
            if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("/StreamingAssets/CampaignMaps/"))
            {
         //       SaveAndLoadGameStory.Instance.SaveGameStory(SaveAndLoadGameStory.Instance.getGamePart().Replace("unfinished", "finished"));
            }


        }
    }

    public AudioSource gameOver_Crushed_Gib_AudioSource;
    public RawImage rawImageFadeToBlack;

    public GameObject gameOver_Crushed_Canvas;

    public RawImage gameOver_flashWhiteRawImage;

    public void GameOver_Crushed()
    {
                if(missionEndEvent == false)
        {
            ScreenShake2.Instance.Shake(0.8f);
            missionEndEvent = true;

            //fade out of all sounds


            //disable player
            PlayerMovementBasic.Instance.enabled = false;
            PlayerMovementBasic.Instance.transform.GetComponent<Rigidbody>().isKinematic = true;

            gameOver_Crushed_Canvas.SetActive(true);

            //play crush sound
            gameOver_Crushed_Gib_AudioSource.Play();

            //rotate camera/player 45 degrees
            Quaternion dutchAngleRotation = Quaternion.Euler(0, 0, 45f);
            PlayerMovementBasic.Instance.transform.rotation *= dutchAngleRotation;



        // Create a flashing loop without interpolation
        gameOver_flashWhiteRawImage.color = new Color(1f,1f,1f,1f);
        gameOver_flashWhiteRawImage.DOFade(0f, 0.1f).SetLoops(5, LoopType.Yoyo).SetEase(Ease.Unset).OnComplete(() => gameOver_flashWhiteRawImage.DOFade(0f, 0.1f));

            //fade to black... on fade to black complete... call level reset event
             rawImageFadeToBlack.DOFade(1, 5f).OnComplete(() => ResetLevelManager.Instance.ResetLevel());
        }
        


    }


    public void ResetValues()
    {
        DOTween.Kill(rawImageFadeToBlack.transform); 

        gameOver_Crushed_Canvas.SetActive(false);


      
    }


    public void GameOver_FallVoid()
    {
                if(missionEndEvent == false)
        {
            missionEndEvent = true;

            //fade out of all sounds


            //fade to black... on fade to black complete... call level reset event
            rawImageFadeToBlack.DOFade(1, 4f).OnComplete(() => ResetLevelManager.Instance.ResetLevel());
        }
        


    }

    public void GameOver()
    {
        if(missionEndEvent == false)
        {
            audioMixer.DOSetFloat("Media", -80f, 1f);
            VLC_GlobalMediaPlayer.Instance.SetVolume(0);

            missionEndEvent = true;
        }
    }

    public void RestartScene()
    {
            //restart scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }

 
}
