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

    public Character owner;

    [Server]
    public void Init(int pDamage, int bDamage, float projSpeed) {
        playerDamage = pDamage;
        buildingDamage = bDamage;
        speed = projSpeed;
    }

    [Server]
    public void MoveProjectile() {
        transform.position += transform.forward * speed;
    }

    [Server]
    void OnTriggerEnter(Collider other) {
        // Apply damage to player or building
        

        if (other.CompareTag("Player")) {
            other.GetComponent<Character>().vitalStats.health -= playerDamage;
            // Despawn bullet
            despawn();

        } 
        else if (other.CompareTag("Block")) {
            Block block = other.GetComponent<Block>();
            block.health -= buildingDamage;
            block.AnimateHit(owner.rotation);
            despawn();
        }
        
        

    }

    void despawn(){
        owner.projectiles.Remove(this);
        NetworkServer.Destroy(gameObject);
    }
}
