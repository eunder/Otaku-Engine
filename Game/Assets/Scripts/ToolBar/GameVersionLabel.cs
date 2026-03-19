using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameVersionLabel : MonoBehaviour
{
    public TextMeshProUGUI gameVersionLabel;
    // Start is called before the first frame update
    void Start()
    {
        gameVersionLabel.text = "Version: " + Application.version;
    }

}
