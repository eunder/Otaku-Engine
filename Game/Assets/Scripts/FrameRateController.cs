using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    public int targetFrameRate = 5;

    void Update()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}