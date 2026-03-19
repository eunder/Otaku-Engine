using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UploadingScreen : MonoBehaviour
{
    public RawImage loadingImage;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI noteText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TurnOn()
    {
        loadingImage.gameObject.SetActive(true);
        loadingText.gameObject.SetActive(true);
        noteText.gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        loadingImage.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);
        noteText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void UpdateMessage(string updateMessage)
    {
        loadingText.text = updateMessage;
    }
}
