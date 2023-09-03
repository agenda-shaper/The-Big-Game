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

    public MeshFilter meshFilter;

    public GameObject meshObject;

    public int despawnDuration;

    [Server]
    public void Init(int pDamage, int bDamage, float projSpeed, string objFilename, int duration) {
        playerDamage = pDamage;
        buildingDamage = bDamage;
        speed = projSpeed;
        despawnDuration = duration;
        Mesh mesh = LoadBulletMesh(objFilename);
        if (mesh != null) {
            meshFilter.mesh = mesh;
            meshObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        } else {
            Debug.LogError("Mesh not found");
        }
    }

    private Mesh LoadBulletMesh(string meshSource)
    {
        return Resources.Load<Mesh>("Meshes/" + meshSource);
    }


    [Server]
    public void MoveProjectile() {
        if (despawnDuration > 0) {
            transform.position += transform.forward * speed;
            despawnDuration-=1;
        } else {
            despawn();
        }
        
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
