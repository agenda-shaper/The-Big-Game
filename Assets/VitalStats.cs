using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class VitalStats : NetworkBehaviour
{
    [SyncVar] public float health = 255;
    [SyncVar] public float hunger = 255;
    [SyncVar] public float cold = 255;
    [SyncVar] public float radiation = 255;
    [SyncVar] public float energy = 255;

    [Server]
    public void HandleVitals(){
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
            // die
        }

    }
}