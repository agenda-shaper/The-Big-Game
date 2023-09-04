using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class VitalStats : NetworkBehaviour
{
    public Character player;
    [SyncVar] public float health = 255;
    [SyncVar] public float hunger = 255;
    [SyncVar] public float cold = 255;
    [SyncVar] public float radiation = 255;
    [SyncVar] public float energy = 255;

    [Server]
    public void HandleVitals(){
        energy = 255;
        if (hunger <= 0){
            health -= 0.02f;
        }
        if (cold <= 0){
            health -= 0.02f;
        }
        if (radiation <= 0){
            health -= 0.02f;
        }
                
        if (health <= 0){
            if (player.immortal){
                health = 255;
                hunger = 255;
                cold = 255;
                radiation = 255;
                energy = 255;
            } else {
                // die
                Debug.Log("dead");
            }
            
        }

    }
}