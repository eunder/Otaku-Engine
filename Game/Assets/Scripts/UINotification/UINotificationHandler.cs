using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UINotificationHandler : MonoBehaviour
{

    private static UINotificationHandler _instance;
    public static UINotificationHandler Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }



    public GameObject notificationsPanel;
    public GameObject notificationPanel_Prefab;


    public enum NotificationStateType{silent ,ping, saved, error, uploadedmap};


    public AudioSource notification_AudioSource;
    public AudioClip notification_ping_AudioClip;
    public AudioClip notification_saved_AudioClip;
    public AudioClip notification_error_AudioClip;
    public AudioClip notification_uploadedMap_AudioClip;

    // Update is called once per frame
    void Update()
    {
     //   if(Input.GetKeyDown(KeyCode.L))
     //   {
     //       SpawnNotification("A test!");
            //spawn in canvas object
     //   }
    }


    public void SpawnNotification(string notiText, NotificationStateType notifState = NotificationStateType.silent)
    {
        GameObject notiPanel = Instantiate(notificationPanel_Prefab, new Vector3(0,0,0), Quaternion.identity);
        notiPanel.GetComponentInChildren<TextMeshProUGUI>().text = notiText;
        notiPanel.transform.SetParent(notificationsPanel.transform);
        notiPanel.GetComponent<Animator>().Rebind();
        notiPanel.GetComponent<Animator>().Play("UINotification1",-1,0f);


        if(notifState == NotificationStateType.ping)
        {
            notification_AudioSource.PlayOneShot(notification_ping_AudioClip);
        }

        if(notifState == NotificationStateType.saved)
        {
            notification_AudioSource.PlayOneShot(notification_saved_AudioClip);
        }

        if(notifState == NotificationStateType.error)
        {
            notification_AudioSource.PlayOneShot(notification_error_AudioClip);
        }

        if(notifState == NotificationStateType.uploadedmap)
        {
            notification_AudioSource.PlayOneShot(notification_uploadedMap_AudioClip);
        }
    }


}
