using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorkshopDropDownAgeRatingMatureInfoScreen : MonoBehaviour
{

    public TMP_Dropdown ageRating_DropDown;
    public GameObject matureInfoWindow;

    // Update is called once per frame
    public void CheckIfMatureWasPickedToShowInfoWindow()
    {
        if(ageRating_DropDown.options[ageRating_DropDown.value].text == "Mature")
        {
            matureInfoWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
        }
    }
}
