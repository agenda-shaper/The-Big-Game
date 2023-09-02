using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour {

    [SyncVar]
    public int playerDamage;

    [SyncVar]
    public int buildingDamage;

    [SyncVar]
    public float speed;

    public Rigidbody rb;

    [Server]
    public void Init(int pDamage, int bDamage, float projSpeed) {
        playerDamage = pDamage;
        buildingDamage = bDamage;
        speed = projSpeed;
    }

    [Server]
    public void MoveProjectile() {
        rb.velocity = transform.forward * speed;
    }
}
