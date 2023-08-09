using UnityEngine;

public class player : MonoBehaviour
{
    public GameObject rotatingObject; // Reference to the object you want to rotate
    public float moveSpeed = 5.0f;
    public float sprintMultiplier = 1.5f;

    private void Update()
    {
        // Movement of the entire player object
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            moveDirection *= sprintMultiplier;
        }
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Rotation of the linked object only
        if (rotatingObject != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 objectScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 directionToMouse = mousePosition - objectScreenPosition;

            float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg - 90;
            rotatingObject.transform.rotation = Quaternion.Euler(0, -angle, 0);
        }
        else
        {
            Debug.LogWarning("Rotating object is not assigned. Please link the object you want to rotate in the Inspector.");
        }
    }
}
