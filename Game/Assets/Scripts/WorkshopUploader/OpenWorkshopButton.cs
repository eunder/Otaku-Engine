using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWorkshopButton : MonoBehaviour
{

    public void OpenSteamOverlayWorkshop()
    {
         Steamworks.SteamFriends.OpenWebOverlay("https://steamcommunity.com//workshop/browse?appid=2002540");
    }

}
