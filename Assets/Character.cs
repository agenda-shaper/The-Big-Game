using System.Reflection.Emit;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Character : NetworkBehaviour
{

    [SyncVar]
    public GameObject character;

    [SyncVar]
    public bool immortal = true;

    [SyncVar]
    public Inventory inventory;

    public Engine engine;
    
    public CharacterController controller;

    public VitalStats vitalStats;

    public LocalPlayer localPlayer;

    public Camera playerCamera;

    public List<Projectile> projectiles;

    [SyncVar]
    public Vector3 cameraOffset = new Vector3(0, 15, 0); // Adjust this for your desired offset


    [SyncVar]
    public float movementSpeed = 0.04f;
    
    [SyncVar]
    public float sprintMultiplier = 1.5f;

    public float rotation;

    public int movingTo = 0; // 0 - idle | 1 - left | 2 - right | 4 - down | 8 - up | 5 - left + down | 6 - right + down | 9 - left + up | 10 - right + up
    public bool isSprinting = false;



    private void Start()
    {
        playerCamera = localPlayer.playerCamera;
        inventory.centerSlots = localPlayer.centerSlots;
        if(playerCamera) playerCamera.enabled = false;
        //localPlayer.buildingManager.startBuilding();

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
        if (rotation < 0) {
            rotation += 360;
        }
        character.transform.rotation = Quaternion.Euler(0, -rotation, 0);

        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.HandleActionSpace(1);
        }

        // Handle 'F' key for action space object 2
        if (Input.GetKeyDown(KeyCode.F))
        {
            inventory.HandleActionSpace(2);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (localPlayer.buildingManager.isBuilding){
                localPlayer.buildingManager.RotateObject();
            }
        }

        if (localPlayer.buildingManager.isBuilding){
            localPlayer.buildingManager.updatePosition(this);
        }
        // if (Input.GetMouseButton(0)) // Left Mouse Button Hold
        // {
        //     if (inventory.actionHandler.isActing && inventory.actionHandler.actionContinous){
        //         inventory.actionHandler.HandleUseAction();
        //     }
        // }
        if (Input.GetMouseButtonDown(0)) // Left Click
        {
            // Code for left click
            if (inventory.highlightedSlot != -1) {
                // inventory handling
                // skip this
                return;
            }

            if (localPlayer.buildingManager.isBuilding){
                BuildBlock(localPlayer.buildingManager);
            } else if (inventory.actionHandler.isActing){
                inventory.actionHandler.HandleUseAction();
            }
            
        }

        if (Input.GetMouseButtonDown(1)) // Right Click
        {
            // Code for right click
        }

        



    }

    public void BuildBlock(BuildManager buildManager){
        // check and verify the positions

        engine.blockManager.SpawnBlock(buildManager.x, buildManager.y, buildManager.slotItem.item.id);

        // remove 1 block
        // do all the inventory checking
    }

    [Server]
    public void HandlePlayer(){
        vitalStats.HandleVitals();
        HandleMovement();
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
            if (vitalStats.energy > 0){
                moveDirection *= sprintMultiplier;
                vitalStats.energy-=0.5f;
            }
            
        }

        float gravity = -50.0f;  // High negative value for gravity
        moveDirection.y = gravity;  // Apply gravity
        controller.Move(moveDirection * movementSpeed);

    }
}