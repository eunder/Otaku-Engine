using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    
    public void UnlockAchivement(string id)
    {
        try
        {
        var ach = new Steamworks.Data.Achievement(id);
        ach.Trigger();
        }
        catch
        {
            Debug.Log("error, not connected to steam?");
        }
    }
}
