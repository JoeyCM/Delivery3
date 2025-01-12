using UnityEngine;
using UnityEngine.EventSystems; // Required to detect UI interactions

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float movementSpeed = 5f;
    public float verticalSpeed = 5f;

    private float pitch = 0f; // Vertical rotation
    private float yaw = 0f; // Horizontal rotation
    private bool canMove = true; // Track whether movement input is allowed

    void Update()
    {
        // If input is disabled, skip the camera movement
        if (!canMove)
        {
            return;
        }

        // Check if the mouse is over a UI element before processing input
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Skip camera movement if interacting with UI
        }

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Prevent flipping when looking up/down

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // WASD movement
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical"); // W/S or Up/Down

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        transform.position += moveDirection * movementSpeed * Time.deltaTime;

        // Vertical movement with Shift (down) and Space (up)
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * verticalSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += Vector3.down * verticalSpeed * Time.deltaTime;
        }
    }

    // Method to disable input when the settings panel is active
    public void DisableInput()
    {
        canMove = false;
    }

    // Method to enable input when the settings panel is not active
    public void EnableInput()
    {
        canMove = true;
    }
}
