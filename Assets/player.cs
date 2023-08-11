using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{

    [SyncVar]
    public float moveSpeed = 100f;
    [SyncVar]
    public float sprintMultiplier = 1.5f;

    public int movingTo; // 0 - idle | 1 - left | 2 - right | 4 - down | 8 - up | 5 - left + down | 6 - right + down | 9 - left + up | 10 - right + up
    public bool isSprinting;


    private void Update()
    {
        if (!isLocalPlayer) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        movingTo = 0;
        if (horizontalInput < 0) movingTo += 1; // Left
        if (horizontalInput > 0) movingTo += 2; // Right
        if (verticalInput < 0) movingTo += 4; // Down
        if (verticalInput > 0) movingTo += 8; // Up

        isSprinting = Input.GetKey(KeyCode.LeftShift);
        Debug.Log(isSprinting);
        Debug.Log(movingTo);
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

        transform.Translate(moveDirection * moveSpeed, Space.World);
    }
}
