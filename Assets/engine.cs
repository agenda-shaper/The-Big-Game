using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Threading;






public class Engine : NetworkBehaviour
{

  public List<Character> playerList = new List<Character>();
  public CustomNetworkManager networkManager; // Assign in the Inspector
  public Vector3 spawnPoint = new Vector3(0, 1, 0);
  public BlockManager blockManager;

  public GameObject localPlayerPrefab;




  public void AddPlayer(Character player) {
    playerList.Add(player);
    Debug.Log("Player added. Total players: " + playerList.Count);

  }

  void FixedUpdate()
  {
    // Check if this instance is running on the server
    if (!isServer) return;

    Tick();
  }

  void Tick(){
    foreach (Character player in playerList)
    {
      player.HandleMovement(); // You would define this method in your Player class
      if (player.inventory.slots.Count < 8){
        player.inventory.SpawnNewSlot();
        player.inventory.AddItemToSlot(player.inventory.slots.Count-1,blockManager.GetItemById(player.inventory.slots.Count+20),1);
        //player.inventory.DropItem(player.inventory.slots.Count-1);
      }

    }
  }

  void Start(){
    Debug.Log("Starting");
    networkManager.StartHost();
    blockManager.LoadItemDatabase();

    blockManager.SpawnBlock(2, 3, 29);
    blockManager.SpawnBlock(3, 2, 29);
    blockManager.SpawnBlock(3, 3, 29);


    //networkManager.StartClient();
    //networkManager.OnClientConnect();
  }


  void OnApplicationQuit()
  {
    //
  }

}