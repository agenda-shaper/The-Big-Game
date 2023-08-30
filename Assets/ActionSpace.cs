using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpace : MonoBehaviour
{
    public Character player;

   

    // void OnTriggerStay(Collider other)
    // {
    //     // Code to execute when an object is inside the trigger
    //     //Debug.Log("Object is inside the trigger");
    // }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ActionSpace") && !player.inventory.actionSpaceObjects.Contains(other.gameObject))
        {
            player.inventory.actionSpaceObjects.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ActionSpace") && player.inventory.actionSpaceObjects.Contains(other.gameObject))
        {
            player.inventory.actionSpaceObjects.Remove(other.gameObject);
        }
    }

}
