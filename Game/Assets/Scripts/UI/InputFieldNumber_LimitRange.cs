using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldNumber_LimitRange : MonoBehaviour
{
    public TMP_InputField inputField;
    public float minValue = 0f;
    public float maxValue = 1f;

    private void Start()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnInputValueChanged);
        }
    }

    private void OnInputValueChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            float floatValue;
            if (float.TryParse(value, out floatValue))
            {
                floatValue = Mathf.Clamp(floatValue, minValue, maxValue);
                inputField.text = floatValue.ToString();
            }
            else
            {
                // Reset to min value if parsing fails
                inputField.text = minValue.ToString();
            }
        }
    }
}