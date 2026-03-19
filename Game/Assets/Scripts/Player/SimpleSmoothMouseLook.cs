using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSmoothMouseLook : MonoBehaviour
{
    private static SimpleSmoothMouseLook _instance;
    public static SimpleSmoothMouseLook Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    
    public bool wheelPickerIsTurnedOn = false;

  public float sensitivity = 2.0f;
    public float minimumX = -90.0f;
    public float maximumX = 90.0f;

    private float rotationX = 0.0f;

    public bool invertYAxis = false;

    public  Rigidbody playerRigidbody;
    void Start()
    {
        if(EscapeToggleToolBar.toolBarisOpened == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public float smoothing = 2.0f;

    private Vector2 smoothMousePosition;
    private Vector2 currentMousePosition;
    private Vector2 deltaMousePosition;

    private void Update()
    {
        if(wheelPickerIsTurnedOn != true)
        {
        // Get raw mouse input
        currentMousePosition = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
      

        // Smooth the mouse input
        deltaMousePosition = Vector2.Scale(currentMousePosition, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothMousePosition.x = Mathf.Lerp(smoothMousePosition.x, deltaMousePosition.x, 1f / smoothing);
        smoothMousePosition.y = Mathf.Lerp(smoothMousePosition.y, deltaMousePosition.y, 1f / smoothing);


        // Calculate rotation
        if(invertYAxis == false)
        {
            rotationX -= smoothMousePosition.y;
        }
        else
        {
            rotationX += smoothMousePosition.y;
        }


        rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f); // Clamp vertical rotation to avoid flipping

        // Rotate the player's body (or camera parent) horizontally
        playerRigidbody.transform.Rotate(Vector3.up * smoothMousePosition.x);

        // Rotate the camera vertically
        transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
      }
      
    }

    public void ResetRotationX()
    {
        rotationX = 0.0f;
    }
}