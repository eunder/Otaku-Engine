using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShaderPickingManager : MonoBehaviour
{

    private static ShaderPickingManager _instance;
    public static ShaderPickingManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }



    public List<GameObject> shaderButtonList = new List<GameObject>();

    
    public void HighlightCurrentSelectedShader()
    {
        //unhighlight all
        foreach(GameObject button in shaderButtonList)
        {
            button.GetComponent<Image>().color = new Color(1,1,1,0.085f);
        }

        //if a matching shader name is found, highlight it
        foreach(GameObject button in shaderButtonList)
        {
            if(ItemEditStateMachine.Instance.currentObjectEditing)
            {
                if(ItemEditStateMachine.Instance.currentObjectEditing.GetComponent<PosterMeshCreator>())
                {
                    if(ItemEditStateMachine.Instance.currentObjectEditing.GetComponent<PosterMeshCreator>().shaderName == button.GetComponent<ShaderPicking_Button>().shaderName)
                    {
                        button.GetComponent<Image>().color = new Color(1,1,1, 0.6f);
                        break;
                    }
                }
            }
        }

    }

}
