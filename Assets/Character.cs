using UnityEngine;
using Mirror;


public class Character : NetworkBehaviour
{

    [SyncVar]
    public GameObject character;

    [SyncVar]
    public Inventory inventory;


    public CharacterController controller;

    public LocalPlayer localPlayer;

    public Camera playerCamera;

    [SyncVar]
    public Vector3 cameraOffset = new Vector3(0, 15, 0); // Adjust this for your desired offset


    [SyncVar]
    public float movementSpeed = 0.05f;
    
    [SyncVar]
    public float sprintMultiplier = 1.5f;

    public float rotation;

    public int movingTo; // 0 - idle | 1 - left | 2 - right | 4 - down | 8 - up | 5 - left + down | 6 - right + down | 9 - left + up | 10 - right + up
    public bool isSprinting;

    private void Start()
    {
        playerCamera = localPlayer.playerCamera;
        inventory.centerSlots = localPlayer.centerSlots;
        if(playerCamera) playerCamera.enabled = false;

    }

    private void Update()
    {
        if (!isLocalPlayer) 
        {
            // Disable the stuff for non-local players

            if(playerCamera) playerCamera.enabled = false;
            return;
        }
        if(playerCamera && !playerCamera.enabled)
        {
            playerCamera.enabled = true;
        }

        playerCamera.transform.position = character.transform.position + cameraOffset;

        float horizontalInput = Input.GetAxis("Horizontal");    
        float verticalInput = Input.GetAxis("Vertical");

        movingTo = 0;
        if (horizontalInput < 0) movingTo += 1; // Left
        if (horizontalInput > 0) movingTo += 2; // Right
        if (verticalInput < 0) movingTo += 4; // Down
        if (verticalInput > 0) movingTo += 8; // Up

        isSprinting = Input.GetKey(KeyCode.LeftShift);
        //Debug.Log(isSprinting);
        //Debug.Log(movingTo);
        Vector3 mousePosition = Input.mousePosition;
        Vector3 centerScreenPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 direction = mousePosition - centerScreenPosition;

        rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //Debug.Log(rotation);
        character.transform.rotation = Quaternion.Euler(0, -rotation, 0);



    }

    [Server]
    public void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        if ((movingTo & 1) != 0) moveDirection.x -= 1; // Left
        if ((movingTo & 2) != 0) moveDirection.x += 1; // Right
        if ((movingTo & 4) != 0) moveDirection.z -= 1; // Down
        if ((movingTo & 8) != 0) moveDirection.z += 1; // Up

        moveDirection.Normalize();

        if (isSprinting)
        {
            moveDirection *= sprintMultiplier;
        }

        controller.Move(moveDirection * movementSpeed);
    }
}