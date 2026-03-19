using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SessionAddressText : MonoBehaviour
{

    private TextMeshProUGUI sessionAdressText;
    // Start is called before the first frame update
    void Start()
    {
        sessionAdressText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        sessionAdressText.text = GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp;
    }
}
