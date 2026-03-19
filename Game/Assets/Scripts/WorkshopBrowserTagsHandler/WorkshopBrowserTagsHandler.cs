using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopBrowserTagsHandler : MonoBehaviour
{
    public Toggle safe_Toggle;
    public Toggle questionable_Toggle;
    public Toggle mature_Toggle;

    public void Start()
    {
        safe_Toggle.isOn = ConfigFileHandler.Instance.configData.Tags.safe;
        questionable_Toggle.isOn = ConfigFileHandler.Instance.configData.Tags.questionable;
        mature_Toggle.isOn = ConfigFileHandler.Instance.configData.Tags.mature;
    }

    public void WorkshopBrowserTagChecked_safe(bool value)
    {
        ConfigFileHandler.Instance.configData.Tags.safe = value;
        ConfigFileHandler.Instance.SaveConfigFile();
    }
    public void WorkshopBrowserTagChecked_questionable(bool value)
    {
        ConfigFileHandler.Instance.configData.Tags.questionable = value;
        ConfigFileHandler.Instance.SaveConfigFile();
    }
    public void WorkshopBrowserTagChecked_mature(bool value)
    {
        ConfigFileHandler.Instance.configData.Tags.mature = value;
        ConfigFileHandler.Instance.SaveConfigFile();
    }
}
