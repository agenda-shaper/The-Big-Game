using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Mirror;
public class Engine : NetworkBehaviour
{

  public List<Character> playerList = new List<Character>();
  public CustomNetworkManager networkManager; // Assign in the Inspector
  public Vector3 spawnPoint = new Vector3(0, 5, 0);


  public void AddPlayer(Character player){
    playerList.Add(player);
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
    }
  }

  void Start(){
    Debug.Log("Starting");
    networkManager.StartHost();
    //networkManager.StartClient();
    //networkManager.OnClientConnect();
  }


  void OnApplicationQuit()
  {
    //
  }

}