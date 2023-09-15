using UnityEngine;

public class Door : MonoBehaviour
{
    public Block block;
    private float rotationSpeed = 500f; // Speed of the door opening
    private float openAngle = 180f; // Angle to which the door opens

    public bool locked;
    public bool isOpen = false;
    private float currentAngle = 0f;

    public void ToggleDoor()
    {
        if (locked){
            return;
        }
        isOpen = !isOpen;
        Debug.Log("toggledoor");
    }
    void Update()
{
    float rotationAmount = rotationSpeed * Time.deltaTime;
    if (isOpen)
    {
        OpenDoor(rotationAmount);
    }
    else
    {
        CloseDoor(rotationAmount);
    }
}

void OpenDoor(float rotationAmount)
{
    if (currentAngle < openAngle)
    {
        RotateDoor(rotationAmount);
        block.collidersNode.SetActive(false); // Disable the collider during opening
        locked = true;
    }
    else
    {
        FinalizeRotation();
        block.collidersNode.SetActive(true); // Enable the collider after opening is complete
        locked = false;
    }
}

void CloseDoor(float rotationAmount)
{
    if (currentAngle > 0)
    {
        RotateDoor(-rotationAmount);
        block.collidersNode.SetActive(false); // Disable the collider during closing
        locked = true;

    }
    else
    {
        FinalizeRotation();
        block.collidersNode.SetActive(true); // Enable the collider after closing is complete
        locked = false;
    }
}

void RotateDoor(float rotationAmount)
{
    block.objectNode.transform.Rotate(Vector3.up, rotationAmount);
    currentAngle += rotationAmount;
}

void FinalizeRotation()
{
    // Round the final rotation to the nearest 90 degrees
    float yRotation = Mathf.Round(block.objectNode.transform.eulerAngles.y / 90f) * 90f;
    block.objectNode.transform.eulerAngles = new Vector3(block.objectNode.transform.eulerAngles.x, yRotation, block.objectNode.transform.eulerAngles.z);
}

}
